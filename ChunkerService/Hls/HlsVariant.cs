namespace ChunkerService.Hls;

/// <summary>
/// Описание параметров для кодирования в HLS (например, разрешение, битрейт).
/// </summary>
public class HlsVariant
{
    public required string Name { get; set; }        // Имя варианта качества (например "360p")
    public int Width { get; set; }          // Ширина кадра
    public int Height { get; set; }         // Высота кадра
    public int VideoBitrate { get; set; }   // Битрейт видео в битах (bit/s)
}