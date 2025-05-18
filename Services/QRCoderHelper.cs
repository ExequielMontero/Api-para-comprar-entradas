using QRCoder;
namespace Api_entradas.Services
{

    public static class QRCoderHelper   
    {
        public static byte[] GenerateQr(string content)
        {
            using var qr = new QRCodeGenerator().CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using var bmp = new PngByteQRCode(qr);
            return bmp.GetGraphic(20);
        }
    }
}
