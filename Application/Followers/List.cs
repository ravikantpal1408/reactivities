using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Profiles;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    public class List
    {
        public class Query : IRequest<List<Profiles.Profiles>>
        {
            public string Username { get; set; }
            public string Predicate { get; set; } // this property is the part of query string
        }

        public class Handler : IRequestHandler<Query, List<Profiles.Profiles>>
        {
            private readonly DataContext _context;
            private readonly IProfileReader _profileReader;

            public Handler(DataContext context, IProfileReader profileReader)
            {
                _context = context;
                _profileReader = profileReader;
            }

            public async Task<List<Profiles.Profiles>> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = _context.Followings.AsQueryable();

                var userFollowings = new List<UserFollowing>();
                var profiles = new List<Profiles.Profiles>();

                switch (request.Predicate)
                {
                    case "followers":
                    {
                        userFollowings = await queryable.Where(x =>
                            x.Target.UserName == request.Username).ToListAsync(cancellationToken: cancellationToken);

                        foreach (var follower in userFollowings)
                        {
                            profiles.Add(await _profileReader.ReadProfile(follower.Observer.UserName));
                        }

                        break;
                    }
                    case "following":
                    {
                        userFollowings = await queryable.Where(x =>
                            x.Observer.UserName == request.Username).ToListAsync(cancellationToken: cancellationToken);

                        foreach (var follower in userFollowings)
                        {
                            profiles.Add(await _profileReader.ReadProfile(follower.Target.UserName));
                        }

                        break;
                    }
                }

                return profiles;
            }
        }
    }
}