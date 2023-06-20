using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Activities
{
    public class Edit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Activity activity { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext context;
            private readonly IMapper mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var dbActivity = await context.Activities.FindAsync(request.activity.Id);
                if (dbActivity == null) return null;
                mapper.Map(request.activity, dbActivity);
                var result = await context.SaveChangesAsync() > 0;
                if (!result) return Result<Unit>.Failure("Cant edit");
                return Result<Unit>.Success(Unit.Value);

            }
        }
    }
}
