namespace _02.SliceFile
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class StartUp
    {
        private const int BufferLength = 4096;

        public static void Main()
        {
            string videoPath = Console.ReadLine();

            string destination = Console.ReadLine();

            int pieces = int.Parse(Console.ReadLine());

            SliceFileAsync(videoPath, destination, pieces);

            string input = Console.ReadLine();

            while (input != "exit")
            {
                input = Console.ReadLine();
            }
        }

        private static void SliceFileAsync(string sourceFile, string destinationPath, int parts)
        {
            Task.Run(() => 
            {
                SliceFile(sourceFile, destinationPath, parts);
            });
        }

        private static void SliceFile(string sourceFile, string destinationPath, int parts)
        {
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            using (var source = new FileStream(sourceFile, FileMode.Open))
            {
                FileInfo fileInfo = new FileInfo(sourceFile);

                long partLength = (source.Length / parts) + 1;
                long currentByte = 0;

                for (int currentPart = 1; currentPart <= parts; currentPart++)
                {
                    string filePath = string.Format($"{destinationPath}/Part-{currentPart}{fileInfo.Extension}");

                    using (var destination = new FileStream(filePath, FileMode.Create))
                    {
                        byte[] buffer = new byte[BufferLength];

                        while (currentByte <= partLength * currentPart)
                        {
                            int readBytesCount = source.Read(buffer, 0, buffer.Length);

                            if (readBytesCount == 0)
                            {
                                break;
                            }

                            destination.Write(buffer, 0, buffer.Length);
                            currentByte += readBytesCount;
                        }
                    }
                }
            }

            Console.WriteLine("Slice complete.");
        }
    }
}
