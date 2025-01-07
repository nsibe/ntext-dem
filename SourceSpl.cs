using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATOParser
{
    class SourceSpl
    { 
        public void SplitSource(string inputText, RichTextBox tb1)
        {
            string[] splitArray = inputText.Split(new string[] { "//" }, StringSplitOptions.None);
            string[] splitLines = inputText.Split(new string[] { "\n" }, StringSplitOptions.None);
            List<string> array = new List<string>(splitArray);
            List<string> lines = new List<string>(splitLines);

            List<string> keywords = new List<string>();
            keywords.Add("AMSNLOC");
            keywords.Add("ROUTE");
            keywords.Add("MSNACFT");
            keywords.Add("AMSNDAT");
            keywords.Add("REFTSK");
            keywords.Add("ASACSDAT");

            string startTskCountry = "TSKCNTRY";

            List<string> taskingItems = new List<string>();
            bool taskingItemsBegin = false;
            bool taskingItemsEnd = false;

            foreach(string item in lines)
            {
                if(item.StartsWith(startTskCountry))
                {
                    taskingItems.Add(item);
                    taskingItemsBegin = true;
                }

                if(taskingItemsBegin && (!taskingItemsEnd))
                {
                    if(!item.StartsWith("TASKUNIT"))
                    {
                        if (!item.StartsWith(startTskCountry))
                        { 
                            taskingItems.Add(item);
                        }
                    }
                    else
                    {
                        taskingItemsEnd = true;
                    }
                }
            }

            foreach(string item in taskingItems)
            {
                tb1.AppendText(item + "\n");
            }

        }
    }
}
