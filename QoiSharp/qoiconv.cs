using StbImageWriteSharp;

namespace QoiSharp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Qoi.Qoi qoi = new Qoi.Qoi();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-help")
                {
                    Console.WriteLine("Example: QoiSharp.exe -i image.qoi -o image.png");
                    break;
                }
                if (args[i] == "-i")
                {
                    if (i + 1 >= args.Length)
                    {
                        throw new Exception();
                        break;
                    }
                    else
                    {
                        var (pixels, width, height, channels) = qoi.Decode(args[i + 1]);

                        if (i + 2 >= args.Length || args[i + 2] != "-o")
                        {
                            throw new Exception();
                            break;
                        }
                        else
                        {
                            if (i + 3 >= args.Length)
                            {
                                throw new Exception();
                                break;
                            }
                            else
                            {
                                using (Stream stream = new FileStream(args[i + 3], FileMode.Create, FileAccess.ReadWrite))
                                {
                                    ImageWriter writer = new ImageWriter();
                                    if (channels == 4)
                                    {
                                        writer.WritePng(pixels, width, height, ColorComponents.RedGreenBlueAlpha, stream);
                                        stream.Close();
                                    }
                                    else if (channels == 3)
                                    {
                                        writer.WritePng(pixels, width, height, ColorComponents.RedGreenBlue, stream);
                                        stream.Close();
                                    }
                                    stream.Close();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}