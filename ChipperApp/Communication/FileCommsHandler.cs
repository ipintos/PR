using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public class FileCommsHandler
    {
        private readonly ConversionHandler _conversionHandler;
        private readonly FileHandler _fileHandler;
        private readonly FileStreamHandler _fileStreamHandler;
        private readonly SocketHelper _socketHelper;

        public FileCommsHandler(SocketHelper socket)
        {
            _conversionHandler = new ConversionHandler();
            _fileHandler = new FileHandler();
            _fileStreamHandler = new FileStreamHandler();
            _socketHelper = socket;
        }

        public void SendFile(string path)
        {
            if (_fileHandler.FileExists(path))
            {
                var fileName = _fileHandler.GetFileName(path);
                _socketHelper.Send(_conversionHandler.ConvertIntToBytes(fileName.Length));
                _socketHelper.Send(_conversionHandler.ConvertStringToBytes(fileName));

                long fileSize = _fileHandler.GetFileSize(path);
                var convertedFileSize = _conversionHandler.ConvertLongToBytes(fileSize);
                _socketHelper.Send(convertedFileSize);
                SendFileWithStream(fileSize, path);
            }
        }

        public void ReceiveFile()
        {
            int fileNameSize = _conversionHandler.ConvertBytesToInt(_socketHelper.Receive(Protocol.HEADER_DATA_SIZE));
            string fileName = _conversionHandler.ConvertBytesToString(_socketHelper.Receive(fileNameSize));
            long fileSize = _conversionHandler.ConvertBytesToLong(_socketHelper.Receive(Protocol.FIXED_FILE_SIZE));
            ReceiveFileWithStreams(fileSize, fileName);
        }

        private void SendFileWithStream(long fileSize, string path)
        {
            long fileParts = Protocol.CalculateFileParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == fileParts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = _fileStreamHandler.Read(path, offset, lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = _fileStreamHandler.Read(path, offset, Protocol.MAX_PACKET_SIZE);
                    offset += Protocol.MAX_PACKET_SIZE;
                }

                _socketHelper.Send(data);
                currentPart++;
            }
        }

        private void ReceiveFileWithStreams(long fileSize, string fileName)
        {
            long fileParts = Protocol.CalculateFileParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == fileParts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = _socketHelper.Receive(lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = _socketHelper.Receive(Protocol.MAX_PACKET_SIZE);
                    offset += Protocol.MAX_PACKET_SIZE;
                }
                _fileStreamHandler.Write(fileName, data);
                currentPart++;
            }
        }
    }
}
