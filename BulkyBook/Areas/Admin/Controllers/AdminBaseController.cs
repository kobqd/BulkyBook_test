using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    public abstract class AdminBaseController<T> : Controller where T:AdminBaseController<T>
    {
        private IUnitOfWork _unitOfWork;
        

        protected IUnitOfWork unitOfWork => _unitOfWork ?? (_unitOfWork = HttpContext?.RequestServices.GetService<IUnitOfWork>());
    }
}
