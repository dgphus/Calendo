﻿using CalendoApplication.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoApplication.Utils.Pagination
{
    public class PaginatedList<TEntity> : IPaginatedList<TEntity>
    {
        public IReadOnlyCollection<TEntity> Items { get; private set; }

        // Thuộc tính để lưu trữ tổng số phần tử
        public int TotalItems { get; private set; }

        // Thuộc tính để lưu trữ số trang hiện tại
        public int CurrentPage { get; private set; }

        // Thuộc tính để lưu trữ tổng số trang
        public int TotalPages { get; private set; }

        // Thuộc tính để lưu trữ số phần tử trên mỗi trang
        public int PageSize { get; private set; }

        // Constructor để khởi tạo danh sách phân trang
        public PaginatedList(IReadOnlyCollection<TEntity> items, int count, int? pageNumber, int? pageSize)
        {
            TotalItems = count;
            CurrentPage = pageNumber ?? 1;
            PageSize = pageSize ?? count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
        }

        // Phương thức để kiểm tra nếu có trang trước đó
        public bool HasPreviousPage => CurrentPage > 1;

        // Phương thức để kiểm tra nếu có trang kế tiếp
        public bool HasNextPage => CurrentPage < TotalPages;

    }
}
