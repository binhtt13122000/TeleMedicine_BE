using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TeleMedicine_BE.Utils
{
    public interface IPagingSupport<T>
    {
        int Count { get; }

        PagingSupport<T> From(IQueryable<T> source);

        PagingSupport<T> GetRange(int pageIndex, int pageSize, Expression<Func<T, object>> selector, int type);

        Paged<TResult> Paginate<TResult>();
    }

    public class PagingSupport<T> : IPagingSupport<T>
    {
        private readonly IMapper _mapper;
        private IQueryable<T> _source;
        private IQueryable<T> _sourcePageSize;
        private int _pageIndex;
        private int _pageSize;

        public PagingSupport(IMapper mapper)
        {
            _mapper = mapper;
        }

        public int Count
        {
            get
            {
                return _source.Count();
            }
        }

        public PagingSupport<T> From(IQueryable<T> source)
        {
            _source = source;
            return this;
        }

        public PagingSupport<T> GetRange(int pageIndex, int pageSize, Expression<Func<T, object>> selector,int type)
        {
            _pageIndex = pageIndex;
            _pageSize = pageSize;
            if(type == 1)
            {
                _sourcePageSize = _source.OrderByDescending(selector).Skip((pageIndex - 1) * pageSize).Take(pageSize);

            } else if(type == 0)
            {
                _sourcePageSize = _source.OrderBy(selector).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            return this;
        }

        public Paged<TResult> Paginate<TResult>()
        {
            int count = Count;
            var pagingVM = new Paged<TResult>()
            {
                TotalCount = Count,
                PageSize = _pageSize,
                TotalPage = (int)Math.Ceiling((double)Count / _pageSize),
                CurrentPage = _pageIndex,
                Content = _sourcePageSize.Select(t => _mapper.Map<TResult>(t))
            };
            if (_pageIndex > 1)
            {
                pagingVM.PreviousPage = _pageIndex - 1;
            }

            if (_pageIndex < count && _pageIndex + 1 <= pagingVM.TotalPage)
            {
                pagingVM.NextPage = _pageIndex + 1;
            }
            else
            {
                pagingVM.NextPage = _pageIndex;
            }

            return pagingVM;
        }
    }

    public class Paged<T>
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public int? NextPage { get; set; }
        public int? PreviousPage { get; set; }
        public IQueryable<T> Content { get; set; }
    }
}
