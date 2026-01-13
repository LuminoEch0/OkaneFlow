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

    public async Task OnGetAsync()
    {
        var ownerId = _currentUser.UserGuid;
        
        // Load Contacts
        var contacts = await _chatService.GetMyContactsAsync();
        Contacts = contacts.Select(c => new ContactVM { 
            UserId = c.TargetUserID, 
            Username = c.Username,
            IsActive = c.TargetUserID == SelectedUserId
        }).ToList();

        // Load Messages if a user is selected
        if (SelectedUserId.HasValue)
        {
            var chatHistory = await _chatService.GetConversationAsync(SelectedUserId.Value);
            Messages = chatHistory.Select(ChatUIMapper.ToVM).ToList();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (SelectedUserId.HasValue && !string.IsNullOrWhiteSpace(NewMessage))
        {
            await _chatService.SendMessageAsync(SelectedUserId.Value, NewMessage);
        }
        return RedirectToPage(new { SelectedUserId });
    }
}
