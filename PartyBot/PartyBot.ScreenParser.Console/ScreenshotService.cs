using Microsoft.Extensions.Hosting;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using Vanara.PInvoke;

namespace PartyBot.ScreenParser.Console
{
    [SupportedOSPlatform("Windows")]
    internal class ScreenshotService : IHostedService
    {
        private Timer? _timer;

        public Task StartAsync(CancellationToken stoppingToken)
        {
            this._timer = new Timer(this.Execute, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._timer?.Change(Timeout.Infinite, Timeout.Infinite);

            return Task.CompletedTask;
        }

        private void Execute(object? state)
        {
            _ = User32.GetWindowRect(User32.GetForegroundWindow(), out var rect);
            using var bitmap = new Bitmap(rect.Width, rect.Height);
            using var graphics = Graphics.FromImage(bitmap);

            graphics.CopyFromScreen(new Point(rect.Left, rect.Top), Point.Empty, rect.Size);
            bitmap.Save(@$"C:\\temp\{Guid.NewGuid()}.jpg", ImageFormat.Jpeg);
        }
    }
}
