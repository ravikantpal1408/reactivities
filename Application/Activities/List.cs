using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        public class ActivityEnvelope
        {
            public List<ActivityDto> Activities { get; set; }
            public int ActivityCount { get; set; }
        }

        public class Query : IRequest<ActivityEnvelope>
        {
            public Query(int? limit, int? offset, bool isGoing, bool isHost, DateTime? startDate)
            {
                Limit = limit;
                Offset = offset;
                IsGoing = isGoing;
                IsHost = isHost;
                StartDate = startDate ?? DateTime.Now;
            }

            public int? Limit { get; set; }
            public int? Offset { get; set; }

            public bool IsGoing { get; set; }

            public bool IsHost { get; set; }

            public DateTime? StartDate { get; set; }
        }

        public class Handler : IRequestHandler<Query, ActivityEnvelope>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<ActivityEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = _context.Activities
                    .Where(x => x.Date >= request.StartDate)
                    .OrderBy(x => x.Date)
                    .AsQueryable();

                // now checking applying the filter
                if (request.IsGoing && !request.IsHost)
                {
                    // applying isHost filter
                    queryable = queryable.Where(x =>
                        x.UserActivities.Any(a => a.AppUser.UserName == _userAccessor.GetCurrentUsername()));
                }

                if (request.IsHost && !request.IsGoing)
                {
                    // applying isGoing filter
                    queryable = queryable.Where(x =>
                        x.UserActivities.Any(a => a.AppUser.UserName == _userAccessor.GetCurrentUsername() && a.IsHost == true));
                }

                var activities = await queryable.Skip(request.Offset ?? 0).Take(request.Limit ?? 0).ToListAsync();

                return new ActivityEnvelope
                {
                    Activities = _mapper.Map<List<Activity>, List<ActivityDto>>(activities),
                    ActivityCount = queryable.Count()
                };

                // var activities = await _context.Activities.ToListAsync(cancellationToken: cancellationToken);

                // return _mapper.Map<List<Activity>, List<ActivityDto>>(activities);
            }
        }
    }
}