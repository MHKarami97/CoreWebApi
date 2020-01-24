﻿using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Data.Contracts;
using Entities.Common;

namespace WebFramework.Api
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class CrudController<TDto, TSelectDto, TEntity, TKey> : BaseController
        where TDto : BaseDto<TDto, TEntity, TKey>, new()
        where TSelectDto : BaseDto<TSelectDto, TEntity, TKey>, new()
        where TEntity : BaseEntity<TKey>, new()
    {
        protected readonly IRepository<TEntity> Repository;
        protected readonly IMapper Mapper;

        public CrudController(IRepository<TEntity> repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ApiResult<List<TSelectDto>>> Get(CancellationToken cancellationToken)
        {
            var list = await Repository.TableNoTracking
                .Where(a => !a.Status.Equals(2))
                .ProjectTo<TSelectDto>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Ok(list);
        }

        [HttpGet("{id:guid}")]
        public virtual async Task<ApiResult<TSelectDto>> Get(TKey id, CancellationToken cancellationToken)
        {
            var dto = await Repository.TableNoTracking
                .Where(a => !a.Status.Equals(2))
                .OrderByDescending(a => a.Version)
                .ProjectTo<TSelectDto>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (dto == null)
                return NotFound();

            return dto;
        }

        [HttpPost]
        public virtual async Task<ApiResult<TSelectDto>> Create(TDto dto, CancellationToken cancellationToken)
        {
            var model = dto.ToEntity(Mapper);

            model.Status = 0;
            model.Version = 1;

            await Repository.AddAsync(model, cancellationToken);

            var resultDto = await Repository.TableNoTracking.ProjectTo<TSelectDto>(Mapper.ConfigurationProvider).SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

            return resultDto;
        }

        [HttpPut]
        public virtual async Task<ApiResult<TSelectDto>> Update(TKey id, TDto dto, CancellationToken cancellationToken)
        {
            var model = await Repository.Table.Where(a => a.Id.Equals(id)).OrderByDescending(a => a.Version).FirstAsync(cancellationToken);

            var newModel = dto.ToEntity(Mapper);

            newModel.Version = model.Version + 1;
            newModel.Status = 0;
            model.Status = 1;

            await Repository.UpdateAsync(model, cancellationToken);

            await Repository.AddAsync(newModel, cancellationToken);

            var resultDto = await Repository.TableNoTracking.ProjectTo<TSelectDto>(Mapper.ConfigurationProvider).SingleOrDefaultAsync(p => p.Id.Equals(newModel.Id), cancellationToken);

            return resultDto;
        }

        [HttpDelete("{id:int}")]
        public virtual async Task<ApiResult> Delete(TKey id, CancellationToken cancellationToken)
        {
            var model = await Repository.Table.Where(a => a.Id.Equals(id)).OrderByDescending(a => a.Version).FirstAsync(cancellationToken);

            model.Status = 2;

            await Repository.UpdateAsync(model, cancellationToken);

            return Ok();
        }
    }

    public class CrudController<TDto, TSelectDto, TEntity> : CrudController<TDto, TSelectDto, TEntity, int>
        where TDto : BaseDto<TDto, TEntity, int>, new()
        where TSelectDto : BaseDto<TSelectDto, TEntity, int>, new()
        where TEntity : BaseEntity<int>, new()
    {
        public CrudController(IRepository<TEntity> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }

    public class CrudController<TDto, TEntity> : CrudController<TDto, TDto, TEntity, int>
        where TDto : BaseDto<TDto, TEntity, int>, new()
        where TEntity : BaseEntity<int>, new()
    {
        public CrudController(IRepository<TEntity> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}