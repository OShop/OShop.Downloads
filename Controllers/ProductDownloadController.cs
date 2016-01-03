﻿using Orchard.ContentManagement;
using Orchard.FileSystems.Media;
using Orchard.MediaLibrary.Models;
using OShop.Downloads.Models;
using OShop.Models;
using OShop.Services;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace OShop.Downloads.Controllers {
    public class ProductDownloadController : Controller {
        private readonly IStorageProvider _storageProvider;
        private readonly IOrdersService _ordersService;
        private readonly IContentManager _contentManager;

        public ProductDownloadController(
            IStorageProvider storageProvider,
            IOrdersService ordersService,
            IContentManager contentManager) {
            _storageProvider = storageProvider;
            _ordersService = ordersService;
            _contentManager = contentManager;
        }

        public ActionResult Download(int Id, string Reference) {
            if(string.IsNullOrWhiteSpace(Reference)) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Missing order parameter");
            }
            var order = _ordersService.GetOrderByReference(Reference);
            if(order == null) {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Order not found");
            }
            else if (order.OrderStatus < OrderStatus.Completed) {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "Order is not completed");
            }
            var detail = order.Details.Where(d => d.Id == Id).FirstOrDefault();
            if(detail == null) {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Order detail not found");
            }
            var product = _contentManager.Get<DownloadableProductPart>(detail.ContentId);
            if (product == null) {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Product not found");
            }
            else if (product.Media == null) {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "No media available for this product");
            }
            else {
                var mediaPart = product.Media.As<MediaPart>();
                try {
                    var storagePath = _storageProvider.GetStoragePath(mediaPart.MediaUrl);
                    var mediaFile = _storageProvider.GetFile(storagePath);
                    return File(mediaFile.OpenRead(), mediaPart.MimeType, mediaPart.FileName);
                }
                catch {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Error retrieving media");
                }
            }
        }
    }
}