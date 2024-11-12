using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Constants;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.Services.ProductService.RoomSpaceService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.ProductController
{
    [ApiController]
    [Route(Routes.ROOMSPACE)]
    public class RoomSpaceController : ControllerBase
    {
        private readonly IRoomSpaceService _roomSpaceService;
        public RoomSpaceController(IRoomSpaceService roomSpaceService)
        {
            _roomSpaceService = roomSpaceService;
        }
        [HttpGet]
        public async Task<IActionResult> GetRoomSpaces([FromQuery] PageInfo pageInfo)
        {
            return new SuccessfulResponse<object>(await _roomSpaceService.GetAllRoomSpaces(pageInfo), (int)HttpStatusCode.OK, "Get RoomSpace successfully").GetResponse();
        }
        [HttpPost()]
        public async Task<IActionResult> CreateRoomSpace([FromForm] RoomSpaceRequest roomSpaceRequest)
        {
            return new SuccessfulResponse<object>(await _roomSpaceService.CreateRoomSpace(roomSpaceRequest), (int)HttpStatusCode.Created, "RoomSpace created successfully").GetResponse();
        }
        [HttpPut("{roomSpaceId}")]
        public async Task<IActionResult> UpdateRoomSpace(Guid roomSpaceId, [FromForm] RoomSpaceRequest roomSpaceRequest)
        {
            return new SuccessfulResponse<object>(await _roomSpaceService.UpdateRoomSpace(roomSpaceId, roomSpaceRequest), (int)HttpStatusCode.OK, "RoomSpace modified successfully").GetResponse();
        }
        [HttpPost("image/{roomSpaceId}")]
        public async Task<IActionResult> ChangeRoomSpaceImage(Guid roomSpaceId, [FromForm] IFormFile roomSpaceRequest)
        {
            await _roomSpaceService.ChangeRoomSpaceImage(roomSpaceId, roomSpaceRequest);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "RoomSpace image changed successfully").GetResponse();
        }
        [HttpDelete("{roomSpaceId}")]
        public async Task<IActionResult> DeleteRoomSpace(Guid roomSpaceId)
        {
            await _roomSpaceService.DeleteRoomSpace(roomSpaceId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "RoomSpace deleted successfully").GetResponse();

        }
    }
}
