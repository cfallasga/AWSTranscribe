using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Amazon;
using Amazon.TranscribeService;
using Amazon.TranscribeService.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace AWSTranscribe.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;


        AmazonTranscribeServiceClient transcribeClient = new AmazonTranscribeServiceClient(
          awsAccessKeyId: "AKIAXPKHMHEO6QYZ4IFT",
          awsSecretAccessKey: "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",
          region: RegionEndpoint.USEast1
        );

        AmazonS3Client s3Client = new AmazonS3Client(
          awsAccessKeyId: "AKIAXPKHMHEO6QYZ4IFT",
          awsSecretAccessKey: "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",
          region: RegionEndpoint.USEast1
        );

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet()
        {
            //string bucketName = "nombre-del-bucket";
            //string filePath = "C:\\test\\test.mp3";

            //PutObjectRequest request1 = new PutObjectRequest()
            //{
            //    BucketName = bucketName,
            //    FilePath = filePath,
            //    Key = Path.GetFileName(filePath)
            //};

            //PutObjectResponse response1 = await s3Client.PutObjectAsync(request1);

            await litasBuckets();
            try
            {
                StartTranscriptionJobRequest request = new StartTranscriptionJobRequest()
                {
                    TranscriptionJobName = "nombre-del-trabajo",
                    LanguageCode = LanguageCode.EsUS,
                    Media = new Media()
                    {
                        MediaFileUri = "s3://mibuktest2/test.mp3"
                    },
                    OutputBucketName = "mibuktest2"
                };
                StartTranscriptionJobResponse response = await transcribeClient.StartTranscriptionJobAsync(request);
                GetTranscriptionJobRequest request3 = new GetTranscriptionJobRequest()
                {
                    TranscriptionJobName = "nombre-del-trabajo"
                };
                GetTranscriptionJobResponse response3 =await transcribeClient.GetTranscriptionJobAsync(request3);
                string transcriptionFileUri = response3.TranscriptionJob.Transcript.TranscriptFileUri;
                TransferUtility transferUtility = new TransferUtility(s3Client);
                string bucketName = "mibuktest2";
                string fileName = "nombre-del-trabajo.json";
                string localFilePath = Path.GetTempFileName();
                transferUtility.Download(localFilePath, bucketName, fileName);
                //string transcriptionText = File.ReadAllText(localFilePath);

               // Console.WriteLine(transcriptionText);

            }
            catch (Exception ex)
            {
                string nsj = ex.Message;
            }
        }

        public async Task litasBuckets()
        {
            ListBucketsResponse response = await s3Client.ListBucketsAsync();
            foreach (S3Bucket bucket in response.Buckets)
            {
                Console.WriteLine($"Bucket name: {bucket.BucketName}, created at {bucket.CreationDate}");
            }
        }
    }
}