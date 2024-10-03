//using Data.Contracts;
//using Entities;
//using Mapster;
//using MediatR;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Mahak.Api.Models.Languages.Commands;

//public class UpdateLanguagesCommand : IRequest<LanguageSelectDto>
//{
//    public IList<LanguageDto> Langs { get; set; }
//}
//public class UpdateLanguagesCommandHandler : IRequestHandler<UpdateLanguagesCommand, LanguageSelectDto>
//{
//    private readonly IRepository<Language> _repository;

//    public UpdateLanguagesCommandHandler(IRepository<Language> repository)
//    {
//        _repository = repository;
//    }
//    public Task<LanguageSelectDto> Handle(UpdateLanguagesCommand request, CancellationToken cancellationToken)
//    {
//        var model = request.Langs.Adapt<IList<Language>>();
//        _repository.UpdateRangeAsync(model, cancellationToken);


//    }
//}
