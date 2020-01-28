using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MyApi.Models;
using Data.Contracts;
using Entities.Post;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class PostsController : CrudController<PostDto, PostSelectDto, Post>
    {
        public PostsController(IRepository<Post> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        public override async Task<ApiResult<PostSelectDto>> Create(PostDto dto, CancellationToken cancellationToken)
        {
            var exist = Repository.TableNoTracking.Any(a => a.Version.Equals(0) && a.Address.Equals(dto.Address));

            if (exist)
                return BadRequest("the address is exist");

            return await base.Create(dto, cancellationToken);
        }
    }
}
