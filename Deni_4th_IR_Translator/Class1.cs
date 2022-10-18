using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Activities;
using System.Net;
using System.IO;


using System.Text.Json;

namespace Deni_4th_IR_Translator
{

    public class SuccessfulRequestModel
    {
        public string original_text { get; set; }
        public string conversion_text { get; set; }
    }
   

    public class Translate:CodeActivity
    {
        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        [RequiredArgument]
        [Category("Input")]
        [DisplayName("Source Language")]
        [Description("Language of sentence that is to be converted")]
        public InArgument<string> SrcLang { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [DisplayName("Conversion Language")]
        [Description("Language sentence is to be converted to")]
        public InArgument<string>CnvrsnLang { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [DisplayName("Sentence")]
        [Description("Text that is to be converted")]
        public InArgument<string> Sentence { get; set; }

        [RequiredArgument]
        [Category("Output")]
        [DisplayName("Response")]
        [Description("Response of API")]
        public OutArgument<string> Response { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            dynamic Result;
            SuccessfulRequestModel model;
            string srcLang = UppercaseFirst(SrcLang.Get(context));
            string cnvrsnLang = UppercaseFirst(CnvrsnLang.Get(context));
            string sentence = Sentence.Get(context);

            //API Details
            string endPoint = "https://text-translation-fairseq-1.ai-sandbox.4th-ir.io/api/v1/sentence";
            string paramdEndPoint = endPoint + "?source_lang=" + srcLang + "&conversion_lang=" + cnvrsnLang;
            string PostData = "{" + String.Format("\"sentence\":\"{0}\"", sentence) + "}";  

            WebRequest request = WebRequest.Create(paramdEndPoint);
            request.Method = "POST";
            request.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(PostData);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = request.GetResponse();

                using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    //esult = reader.Read(1);
                    // JsonObject jsonObject;
                    // model = JsonSerializer.Deserialize<SuccessfulRequestModel>(Result);

                    string text = reader.ReadToEnd();
                    Result = JsonSerializer.Deserialize<SuccessfulRequestModel>(text);
                }
            }

            Response.Set(context, Result.conversion_text);
        }
    }
}
