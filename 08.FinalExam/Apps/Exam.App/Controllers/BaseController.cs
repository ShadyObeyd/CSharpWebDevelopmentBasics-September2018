using Exam.Data;
using SIS.MvcFramework;

namespace Exam.App.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            this.Db = new ExamContext();
        }

        protected ExamContext Db { get; }
    }
}
