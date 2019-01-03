using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using WebApi.Common;

namespace WebApi.Controllers
{
    public class ValuesController : ApiController
    {
        public static ValuesController Instance
        {
            get { return Singleton<ValuesController>.GetInstance(); }
        }

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public string Post([FromBody]string value)
        {
            return "POST" + value;
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }


        private Queue<QueueInfo> ListQueue = new Queue<QueueInfo>();

        public class QueueInfo
        {
            public int listcount { get; set; }
        }

        [HttpPost]
        [Route("api/QueueTest")]
        public string QueueTest()
        {
            int y = Convert.ToInt32(HttpContext.Current.Request["y"]);
            QueueInfo queueInfo = new QueueInfo();

            AddQueue(y);
            //threadStart();

            return queueInfo.listcount.ToString();
        }

        public void AddQueue(int y)
        {
            QueueInfo queueInfo = new QueueInfo();
            queueInfo.listcount = y;
            ListQueue.Enqueue(queueInfo);
        }

        public void Start()
        {
            Thread thread = new Thread(threadStart);
            thread.IsBackground = true;
            thread.Start();
        }

        private void threadStart()
        {
            while (true)
            {
                if (ListQueue.Count > 0)
                {
                    try
                    {
                        ScanQueue();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                else
                {
                    Thread.Sleep(3000);
                }
            }

            //while (ListQueue.Count > 0)
            //{
            //    try
            //    {
            //        ScanQueue();
            //    }
            //    catch (Exception ex)
            //    {
            //        throw;
            //    }
            //}
        }

        private void ScanQueue()
        {
            while (ListQueue.Count > 0)
            {
                try
                {
                    QueueInfo queueInfo = new QueueInfo();

                    queueInfo.listcount = ListQueue.Count;
                    queueInfo = ListQueue.Dequeue();


                    ArrayList list = new ArrayList();

                    for (int i = 0; i < queueInfo.listcount; i++)
                    {
                        list.Add(i);
                        list.Add(i.ToString());
                    }

                    IList<int> ilist = new List<int>();

                    for (int i = 0; i < list.Count; i++)
                    {
                        ilist.Add(Convert.ToInt32(list[i]));
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return;
        }
    }
}
