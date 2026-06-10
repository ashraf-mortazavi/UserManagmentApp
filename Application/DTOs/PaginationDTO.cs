using System;

namespace ManageUsers.Application.DTOs;

public class PaginationDTO : BaseResponse
{
    public int PageNumber { get; set; }

    public int LastPage { get; set; }

    private int totalRecords;
    public int TotalRecords
    {
        get
        {
            return totalRecords;
        }
        set
        {
            totalRecords = value;
            SetLastPage();
        }
    }

    private int pageSize;
    public int PageSize
    {
        get
        {
            return pageSize;
        }
        set
        {
            pageSize = value;
            SetLastPage();
        }
    }

    private void SetLastPage()
    {
        if (PageSize > 0 && TotalRecords > 0)
        {
            LastPage = (int)Math.Ceiling((decimal)TotalRecords / PageSize);
        }
        else if (TotalRecords == 0)
        {
            LastPage = 0;
        }
    }
}
