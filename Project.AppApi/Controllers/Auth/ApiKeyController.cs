using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcCore.Extension.Auth;
using ViewModel;
using ViewModel.Auth;

namespace Project.AppApi.Controllers.Auth
{
    /// <summary>
    /// API Key 管理接口
    /// </summary>
    [ApiExplorerSettings(GroupName = "V1")]
    // 模板项目允许匿名访问以便测试；实际项目请改回 [Authorize(Roles = "Admin")]
    [AllowAnonymous]
    public class ApiKeyController(ApiKeyService apiKeyService) : ControllerBase
    {
        /// <summary>
        /// 创建 API Key
        /// </summary>
        /// <remarks>
        /// 返回的 ApiKey 明文仅在创建时返回一次，后续无法再获取，请务必保存。
        /// API Key 用于服务间调用或第三方接入，通过请求头 X-Api-Key 传入。
        /// </remarks>
        [HttpPost]
        [Route("api/v1/auth/apikey")]
        public ResultModel<ApiKeyResDto> Create([FromBody] ApiKeyCreateDto req)
        {
            var result = new ResultModel<ApiKeyResDto>();

            if (string.IsNullOrEmpty(req?.Name))
            {
                result.Success = false;
                result.Message = "名称不能为空";
                return result;
            }

            var (apiKey, resDto) = apiKeyService.CreateApiKey(req.Name, req.Scopes, req.ExpiresInDays);

            result.Data = resDto;
            result.Data.ApiKey = apiKey;

            return result;
        }

        /// <summary>
        /// 查询所有 API Key 列表
        /// </summary>
        /// <remarks>
        /// 返回列表不含 API Key 明文，仅包含 KeyPrefix（前缀）用于识别。
        /// </remarks>
        [HttpGet]
        [Route("api/v1/auth/apikeys")]
        public ResultModel<List<ApiKeyResDto>> List()
        {
            var result = new ResultModel<List<ApiKeyResDto>>();
            result.Data = apiKeyService.GetAllApiKeys();
            return result;
        }

        /// <summary>
        /// 吊销/删除指定 API Key
        /// </summary>
        [HttpDelete]
        [Route("api/v1/auth/apikey/{id}")]
        public ResultModel Delete(int id)
        {
            var result = new ResultModel();
            bool success = apiKeyService.RevokeApiKey(id);
            result.Success = success;
            result.Message = success ? "已删除" : "未找到该 API Key";
            return result;
        }
    }
}
