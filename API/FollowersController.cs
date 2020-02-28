using System.Collections.Generic;
using System.Threading.Tasks;
using API.Controllers;
using Application.Followers;
using Application.Profiles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API
{
    [Route("api/profiles")]
    public class FollowersController : BaseController
    {
        [HttpPost("{username}/follow")]
        public async Task<ActionResult<Unit>> Follow(string username)
        {
            return await Mediator.Send(new Add.Command {Username = username});
        }

        [HttpDelete("{username}/follow")]
        public async Task<ActionResult<Unit>> UnFollow(string username)
        {
            return await Mediator.Send(new Delete.Command {Username = username});
        }

        [HttpGet("{username}/follow")]
        public async Task<ActionResult<List<Profiles>>> GetFollowings(string username, string predicate)
        {
            return await Mediator.Send(new List.Query {Username = username, Predicate = predicate});
        }
    }
}