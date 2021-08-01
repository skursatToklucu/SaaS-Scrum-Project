using Microsoft.AspNetCore.Http;
using PropTabTabIK.DataAccess.Context;
using PropTabTabIK.Entities.SideEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PropTabTabIK.DataAccess.Repositories.Concrete
{
    public class AllowRequestRepository : SideRepository<AllowRequest>
    {
        private readonly ProjectContext _context;
        private IHttpContextAccessor _httpContext;
        public AllowRequestRepository(ProjectContext context, IHttpContextAccessor httpContext) : base(context, httpContext)
        {
            _httpContext = httpContext;
            _context = context;

        }

        /// <summary>
        /// Toplam talep edilen izin gününü hesaplayan metod
        /// </summary>
        /// <param name="allowRequest"></param>
        /// <returns></returns>
        public int AllowTime(DateTime endDate, DateTime startDate)
        {
            if (endDate != null && startDate != null)
            {

                TimeSpan diff = endDate.Subtract(startDate);
                return diff.Days;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Kalan izin gününü hesaplayan metod
        /// </summary>
        /// <param name="allowRequest"></param>
        /// <returns></returns>
        public int RemainingAllowTime(Guid id)
        {
            AllowRequest allowRequest = GetByID(id);
            int RemainingTime = allowRequest.TotalAllowTime - allowRequest.AllowTime;

            return RemainingTime;
        }


        /// <summary>
        /// Seçilen Tarihler DB de kayıtlı olup olmadığını kontrol eder.Varsa True
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public bool IsContainDate(DateTime startDate, DateTime endDate)
        {
            var dates = new List<DateTime>();

            for (var dt = startDate; dt < endDate; dt = dt.AddDays(1))
            {
                dates.Add(dt);
            }

            foreach (var item in dates)
            {

                if (_context.AllowRequests.Any(x => x.StartDate == item) || _context.AllowRequests.Any(x => x.EndDate == item))
                    return true;

                else
                    return false;
            }

            return false;
        }


        /// <summary>
        /// Onaylanan Talepleri birlestirir
        /// </summary>
        /// <param name="allowRequests"></param>
        /// <returns></returns>
        public AllowRequest MergeAllowRequest(List<AllowRequest> allowRequests)
        {

            AllowRequest newAllowRequest = new AllowRequest();

            newAllowRequest.AllowType = allowRequests[0].AllowType;
            newAllowRequest.StartDate = allowRequests[0].StartDate;
            newAllowRequest.EndDate = allowRequests[allowRequests.Count - 1].EndDate;
            newAllowRequest.EmployeeID = allowRequests[0].EmployeeID;
            foreach (var item in allowRequests)
            {
                newAllowRequest.AllowTime += item.AllowTime;
                if (newAllowRequest.AllowTime > newAllowRequest.TotalAllowTime)
                    newAllowRequest.AllowTime = newAllowRequest.TotalAllowTime;

            }
            newAllowRequest.IssueDate = allowRequests[allowRequests.Count - 1].IssueDate;
            newAllowRequest.State = Core.Enum.State.Confirmed;
            newAllowRequest.RemainingAllowTime = newAllowRequest.TotalAllowTime - newAllowRequest.AllowTime;

            foreach (var item in allowRequests)
            {
                _context.Remove(item);
            }

            return newAllowRequest;
        }
    }
}