using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using Vanara.PInvoke;
using static Vanara.PInvoke.Gdi32;

namespace PartyBot.ScreenParser.Console
{
    [SupportedOSPlatform("Windows")]
    internal class ScreenshotService : IHostedService
    {
        private readonly double _screenScale;
        private readonly ILogger<ScreenshotService> _logger;
        private Timer? _timer;
        private Bitmap? _referenceBitmap;

        public ScreenshotService(IConfiguration configuration, ILogger<ScreenshotService> logger)
        {
            this._screenScale = configuration.GetValue("ScreenScale", 1.00);
            this._logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            this._referenceBitmap = new Bitmap($"{AppDomain.CurrentDomain.BaseDirectory}/Assets/minigame-help.bmp");
            this._timer = new Timer(this.Execute, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._timer?.Change(Timeout.Infinite, Timeout.Infinite);
            this._timer?.Dispose();
            this._referenceBitmap?.Dispose();

            return Task.CompletedTask;
        }

        private void Execute(object? state)
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                if (this._referenceBitmap == null || !User32.GetWindowRect(User32.GetForegroundWindow(), out var rect))
                {
                    return;
                };

                using var bitmap = new Bitmap(rect.Width, rect.Height);
                using var graphics = Graphics.FromImage(bitmap);

                graphics.CopyFromScreen(new Point(rect.Left, rect.Top), Point.Empty, rect.Size);

                var comparisonScore = Compare(bitmap, this._referenceBitmap, new Point(452, 125), 0.95);
                stopwatch.Stop();

                this._logger.LogInformation("Comparison score: {Score}. Time taken: {TimeTaken}", comparisonScore, stopwatch.Elapsed);
            }
            catch (Exception exception)
            {
                this._logger.LogError(exception, "Failed when parsing screen: {Exception}", exception.Message);
            }
        }

        private static double Compare(Bitmap testBitmap, Bitmap referenceBitmap, Point position, double threshold)
        {
            if (testBitmap.Width - position.X - referenceBitmap.Width < 0 || testBitmap.Height - position.Y - referenceBitmap.Height < 0)
            {
                return 0;
            }

            double total = 0;
            for (int horizontalIndex = 0; horizontalIndex < referenceBitmap.Width; horizontalIndex++)
            {
                for (int verticalIndex = 0; verticalIndex < referenceBitmap.Height; verticalIndex++)
                {
                    var testPixel = testBitmap.GetPixel(horizontalIndex + position.X, verticalIndex + position.Y);
                    var referencePixel = referenceBitmap.GetPixel(horizontalIndex, verticalIndex);

                    var redDifference = Math.Abs(testPixel.R - referencePixel.R);
                    var greenDifference = Math.Abs(testPixel.G - referencePixel.G);
                    var blueDifference = Math.Abs(testPixel.B - referencePixel.B);

                    // Can range from 0 to 3 * byte.MaxValue
                    total += redDifference + greenDifference + blueDifference;
                }

                if (GetNormalizedMeanDifference(total, referenceBitmap.Width * referenceBitmap.Height) < threshold)
                {
                    return 0;
                }
            }

            return GetNormalizedMeanDifference(total, referenceBitmap.Width * referenceBitmap.Height);
        }

        private void SaveBitmap(Bitmap bitmap)
        {
            var filename = DateTime.Now.ToString("s")
                .Replace(".", string.Empty)
                .Replace(":", string.Empty)
                .Replace("-", string.Empty)
                .Replace("T", string.Empty);

            bitmap.Save(@$"C:\\temp\{filename}.bmp", ImageFormat.Bmp);
        }

        private static Color[,] EnumerateBitmap(Bitmap bitmap, Rectangle boundingRectangle)
        {
            var result = new Color[boundingRectangle.Width, boundingRectangle.Height];
            for (int horizontalIndex = boundingRectangle.X; horizontalIndex < boundingRectangle.Width; horizontalIndex++)
            {
                for (int verticalIndex = boundingRectangle.Y; verticalIndex < boundingRectangle.Height; verticalIndex++)
                {
                    result[horizontalIndex, verticalIndex] = bitmap.GetPixel(horizontalIndex, verticalIndex);
                }
            }

            return result;
        }

        private static double GetNormalizedMeanDifference(double total, int count)
        {
            var meanDifference = total / count;
            var normalizedMeanDifference = meanDifference / (3 * byte.MaxValue);
            return 1 - normalizedMeanDifference;
        }
    }
}
