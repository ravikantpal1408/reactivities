using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SendComment(Create.Command command)
        {
            // getting user from Hub Context
            var username = GetUsername();

            command.Username = username;
            
            var comment = await _mediator.Send(command);
            
            await Clients.Group(command.ActivityId.ToString()).SendAsync("ReceiveComment", comment);
        }


        private string GetUsername()
        {
            return  Context.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        // groupName is basically a activity id - GUID
        public async Task AddToGroup(string groupName)
        {
            var username = GetUsername();
            // sending a comment to specific group chat
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{username} has joined the group");
        }
        
        
        public async Task RemoveFromGroup(string groupName)
        {
            var username = GetUsername();
            // sending a comment to specific group chat
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{username} has left the group");
        }
    }
}