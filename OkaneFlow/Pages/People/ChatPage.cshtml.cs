using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service.Interface;

namespace OkaneFlow.Pages.People;

public class ChatPageModel : PageModel
{
    private readonly IChatService _chatService;
    private readonly ICurrentUserService _currentUser;

    public ChatPageModel(IChatService chatService, ICurrentUserService currentUser)
    {
        _chatService = chatService;
        _currentUser = currentUser;
    }

    public List<ContactVM> Contacts { get; set; } = [];
    public List<ChatMessageVM> Messages { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public Guid? SelectedUserId { get; set; }

    [BindProperty]
    public string NewMessage { get; set; } = string.Empty;

    [BindProperty]
    public string SearchUsername { get; set; } = string.Empty;

    public string? SearchMessage { get; set; }
    public bool ShowSearchError { get; set; }
    public bool IsBlockedByMe { get; set; }

    public async Task OnGetAsync()
    {
        var ownerId = _currentUser.UserGuid;

        // Load Contacts
        var contacts = await _chatService.GetMyContactsAsync();
        Contacts = contacts.Select(c => new ContactVM
        {
            UserId = c.TargetUserID,
            Username = c.Username,
            IsActive = c.TargetUserID == SelectedUserId
        }).ToList();

        // Load Messages if a user is selected
        if (SelectedUserId.HasValue)
        {
            // Mark conversation as read
            await _chatService.MarkConversationAsReadAsync(SelectedUserId.Value);

            // Check if blocked by me
            IsBlockedByMe = await _chatService.IsUserBlockedByMeAsync(SelectedUserId.Value);

            var chatHistory = await _chatService.GetConversationAsync(SelectedUserId.Value);
            Messages = chatHistory.Select(ChatUIMapper.ToVM).ToList();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (SelectedUserId.HasValue && !string.IsNullOrWhiteSpace(NewMessage))
        {
            try
            {
                await _chatService.SendMessageAsync(SelectedUserId.Value, NewMessage);
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
        }
        return RedirectToPage(new { SelectedUserId });
    }

    public async Task<IActionResult> OnPostSearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchUsername))
        {
            return RedirectToPage();
        }

        var user = await _chatService.SearchUserByExactNameAsync(SearchUsername);
        if (user == null)
        {
            TempData["SearchError"] = "User not found.";
        }
        else if (user.UserID == _currentUser.UserGuid)
        {
            TempData["SearchError"] = "You cannot add yourself.";
        }
        else
        {
            try
            {
                await _chatService.AddContactAsync(SearchUsername);
                TempData["SearchSuccess"] = $"User {SearchUsername} added to contacts.";
            }
            catch (Exception ex)
            {
                TempData["SearchError"] = ex.Message;
            }
        }

        return RedirectToPage(new { SelectedUserId });
    }

    public async Task<IActionResult> OnPostBlockAsync(Guid userId)
    {
        await _chatService.BlockUserAsync(userId);
        return RedirectToPage(new { SelectedUserId = userId });
    }

    public async Task<IActionResult> OnPostUnblockAsync(Guid userId)
    {
        await _chatService.UnblockUserAsync(userId);
        return RedirectToPage(new { SelectedUserId = userId });
    }

    public async Task<JsonResult> OnGetMessagesAsync(Guid targetUserId)
    {
        // Mark as read when polling
        await _chatService.MarkConversationAsReadAsync(targetUserId);

        var chatHistory = await _chatService.GetConversationAsync(targetUserId);
        var vms = chatHistory.Select(ChatUIMapper.ToVM).ToList();
        return new JsonResult(vms);
    }
}
