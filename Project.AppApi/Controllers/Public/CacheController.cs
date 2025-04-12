using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModel;

namespace Project.AppApi.Controllers
{
    /// <summary>
    /// 系统缓存查询
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = false)] // 默认显示
    public class CacheController : BaseController
    {

        #region 可将用户登录信息缓存 JWT可不用
        /// <summary>
        /// 获取所有系统缓存键列表
        /// </summary>
        /// <returns></returns>
        [Route("/api/cache/get-multi")]
        [HttpGet]
        [AllowAnonymous]
        public ResultModel<List<string>> GetList()
        {
            ResultModel<List<string>> resultModel = new ResultModel<List<string>>();
            resultModel.data = MvcCore.Extension.MemoryCacheHelper.GetCacheKeys();
            return resultModel;
        }

        /// <summary>
        /// 重置程序所有系统缓存
        /// </summary>
        /// <returns></returns>
        [Route("/api/cache/reset-system-all")]
        [HttpGet]
        [AllowAnonymous]
        public ResultModel ResetSystemAll()
        {
            ResultModel resultModel = new ResultModel();
            resultModel.success = true;
            int num = 0;
            foreach (var key in MvcCore.Extension.MemoryCacheHelper.GetCacheKeys().ToArray())
            {
                MvcCore.Extension.MemoryCacheHelper.Remove(key);
                num++;
            }

            resultModel.message = "重置所有(" + num + "个)系统缓存成功!";
            return resultModel;
        }

        /// <summary>
        /// 重置程序用户系统缓存
        /// </summary>
        /// <param name="UserIdList">用户id集合</param>
        /// <returns></returns>
        [Route("/api/cache/reset-system-user")]
        [HttpPost]
        [AllowAnonymous]
        public ResultModel ResetSystemUser(List<string> UserIdList)
        {
            ResultModel resultModel = new ResultModel();
            resultModel.success = true;
            int num = 0;
            if (UserIdList != null && UserIdList.Any())
            {
                foreach (var userID in UserIdList)
                {
                    //先擠掉其它登錄用戶
                    //HttpContext.SignOutByUserAsync(userID);
                    num++;
                }
            }
            resultModel.message = "重置(" + num + "个)用户系统缓存成功!";
            return resultModel;
        }
        #endregion

        #region 程序加锁的缓存
        /// <summary>
        /// 获取所有程序缓存键列表
        /// </summary>
        /// <returns></returns>
        [Route("/api/cache/get-project-multi")]
        [HttpGet]
        [AllowAnonymous]
        public ResultModel<List<string>> GetProjectList()
        {
            ResultModel<List<string>> resultModel = new ResultModel<List<string>>();
            resultModel.data = Core.MemoryCacheHelper.GetCacheKeys();
            return resultModel;
        }

        /// <summary>
        /// 重置程序缓存（重置传入缓存）
        /// </summary>
        /// <param name="cacheList"></param>
        /// <returns></returns>
        [Route("/api/cache/reset")]
        [HttpPost]
        [AllowAnonymous]
        public ResultModel Reset(List<string> cacheList)
        {
            ResultModel resultModel = new ResultModel();
            resultModel.success = true;

            int num = 0;
            if (cacheList != null && cacheList.Any())
            {
                foreach (var item in cacheList)
                {
                    Core.MemoryCacheHelper.Remove(item);
                    num++;
                }
            }

            resultModel.message = "重置" + num + "个程序缓存成功!";
            return resultModel;
        }


        /// <summary>
        /// 重置程序缓存（重置传入模糊匹配数据）
        /// </summary>
        /// <param name="cacheKey">模糊匹配数据</param>
        /// <returns></returns>
        [Route("/api/cache/reset-by-Key")]
        [HttpGet]
        [AllowAnonymous]
        public ResultModel ResetByKey(string cacheKey)
        {
            ResultModel resultModel = new ResultModel();
            resultModel.success = true;

            int num = 0;

            //获取所有缓存数据
            List<string> cacheList = Core.MemoryCacheHelper.GetCacheKeys().Where(x => x.Contains(cacheKey)).ToList();

            //清除缓存
            if (cacheList != null && cacheList.Any())
            {
                foreach (var item in cacheList)
                {
                    Core.MemoryCacheHelper.Remove(item);
                    num++;
                }
            }

            resultModel.message = "重置" + num + "个程序缓存成功!";
            return resultModel;
        }

        /// <summary>
        /// 重置程序所有缓存
        /// </summary>
        /// <returns></returns>
        [Route("/api/cache/reset-all")]
        [HttpGet]
        [AllowAnonymous]
        public ResultModel ResetAll()
        {
            ResultModel resultModel = new ResultModel();
            resultModel.success = true;
            int num = 0;
            foreach (var key in Core.MemoryCacheHelper.GetCacheKeys().ToArray())
            {
                Core.MemoryCacheHelper.Remove(key);
                num++;
            }

            resultModel.message = "重置所有(" + num + "个)程序缓存成功!";
            return resultModel;
        }
        #endregion
    }
}