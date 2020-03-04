using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
            public Query(int? limit, int? offset)
            {
                Limit = limit;
                Offset = offset;
            }

            public int? Limit { get; set; }
            public int? Offset { get; set; }
        }

        public class Handler : IRequestHandler<Query, ActivityEnvelope>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<ActivityEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = _context.Activities.AsQueryable();

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