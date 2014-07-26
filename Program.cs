using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
//----------------------------------------------------------------------
//数据库有5个事物。设min_sup=60%，min_conf=80%
//TID                        购买的商品
//T100                     {M, O, N, K, E, Y}
//T200                     {D, O, N, K, E, Y}
//T300                     {M, A, K, E}
//T400                     {M, U, C, K, Y}
//T500                     {C, O, O, K, I, E}
//使用Apriori算法找出所有的频繁项集
//zhangchao 2013-5-31
//6-5  程序中加入剪枝操作
//      程序中加入Init初始化界面函数，Print函数                               
//----------------------------------------------------------------------
namespace Apriori
{
    class Program
    {
        static Dictionary<string, int> c1 = new Dictionary<string, int>();
        static Dictionary<string, int> c2 = new Dictionary<string, int>();
        static Dictionary<string, int> c3 = new Dictionary<string, int>();
        static Dictionary<string, int> l1 = new Dictionary<string, int>();
        static Dictionary<string, int> l2 = new Dictionary<string, int>();
        static Dictionary<string, int> l3 = new Dictionary<string, int>();
 
        static void Main(string[] args)
        {
            //已知数据
            double min_sup = 0.6;
            double min_conf = 0.8;
            ArrayList T100 = new ArrayList { "M", "O", "N", "K", "E", "Y" };
            ArrayList T200 = new ArrayList { "D", "O", "N", "K", "E", "Y" };
            ArrayList T300 = new ArrayList { "M", "A", "K", "E" };
            ArrayList T400 = new ArrayList { "M", "U", "C", "K", "Y" };
            ArrayList T500 = new ArrayList { "C", "O", "O", "K", "I", "E" };

            ArrayList List = new ArrayList { T100, T200, T300, T400, T500 };
            double min_count = List.Count * min_sup;
            for (int i = 0; i < List.Count; i++)
            {
                List[i] = NoRepeat((ArrayList)List[i]);
            }
            foreach (ArrayList L in List)
            {
                AddC1(L);
            }
            l1=GetL(c1, min_count);
            BuildC2(l1,List);
            l2 = GetL(c2, min_count);
            GetC3(l2,List);
            l3 = GetL(c3, min_count);
            Init();
            Console.WriteLine("候选一项集：");
            Print(c1);
            Console.WriteLine("频繁一项集：");
            Print(l1);
            Console.WriteLine("候选二项集：");
            Print(c2);
            Console.WriteLine("频繁二项集：");
            Print(l2);
            Console.WriteLine("候选三项集：");
            Print(c3);
            Console.WriteLine("频繁三项集：");
            Print(l3);
            Console.ReadKey();
        }

        /// <summary>
        /// 元素去重复
        /// </summary>
        public static ArrayList NoRepeat(ArrayList T)
        {
            ArrayList L = new ArrayList();
            for (int i = 0; i < T.Count; i++)
            {
                if (L.Contains(T[i])) { }
                else if (!L.Contains(T[i]))
                {
                    L.Add(T[i]);
                }
            }
            T = L;
            return T;
        }

        /// <summary>
        /// 构建c1
        /// </summary>
        public static void AddC1(ArrayList T)
        {
            for (int i = 0; i < T.Count; i++)
            {
                if (!c1.ContainsKey(T[i].ToString()))
                {
                    c1.Add(T[i].ToString(), 1);
                }
                else if (c1.ContainsKey(T[i].ToString()))
                {
                    c1[T[i].ToString()] = c1[T[i].ToString()] + 1;
                }
            }
        }

        /// <summary>
        /// 构建频繁项集
        /// </summary>
        public static Dictionary<string, int> GetL(Dictionary<string, int> c, double d)
        {
            Dictionary<string, int> l = new Dictionary<string, int>();
            //遍历候选项集中的key，找出key对应的value值大于min_count的数据，存入频繁项集中
            foreach (string obj in c.Keys)
            {
                if (c[obj]>=d)
                {
                    l.Add(obj, c[obj]);
                }
            } 
            return l;
        }

        /// <summary>
        /// 构建c2
        /// </summary>
        public static void BuildC2(Dictionary<string, int> l1,ArrayList List)
        {
            int count = 0;
            //取得所有l1中的元素
            ArrayList elements = new ArrayList();
            foreach (string obj in l1.Keys)
            {
                elements.Add(obj);
            }
            //循环拼接字符串得到c2的key值
            StringBuilder temp;
            for (int i = 0; i < elements.Count; i++)
            {
                string s1 = elements[i].ToString();
                temp = new StringBuilder(s1);
                for (int j = i + 1; j < elements.Count; j++)
                {
                    string s2 = elements[j].ToString();
                    temp.Append(s2);
                    foreach (ArrayList T in List)
                    {
                        //若T中既同时存在s1和s2，则计数器加1
                        if (T.Contains(s1) && T.Contains(s2))
                        {
                            count++;
                        }
                    }
                    //以键值对方式向c2中写入
                    c2.Add(temp.ToString(), count);
                    count = 0;
                    temp = new StringBuilder(s1);
                }
            }
        }

        /// <summary>
        /// 判断一个字符串中是否存在重复的char单元，若存在，去除重复的char元素
        /// </summary>
        public static bool IsExistRepeatElement(StringBuilder sbs,out StringBuilder temp)
        {
            ArrayList sb = new ArrayList();
            for (int i = 0; i < sbs.Length; i++)
            {
                sb.Add(sbs[i]);
            }
            for (int i = 0; i < sb.Count; i++)
            {
                for (int j = 3; j > i; j--)
                {
                    if (sb[i].Equals(sb[j]))
                    {
                        sb.Remove(sb[i]);
                        sb.Sort();
                        temp = new StringBuilder();
                        for (int k = 0; k < sb.Count; k++)
                        {
                            temp.Append(sb[k]); 
                        }
                        return true;
                    }
                }
            }
            temp = new StringBuilder();
            return false;
        }

        /// <summary>
        /// 根据l2得到c3
        /// </summary>
        public static void GetC3(Dictionary<string, int> l2, ArrayList List)
        {
            ArrayList elements = new ArrayList();
            foreach (string obj in l2.Keys)
            {
                //将l2中的key存放在elements中
                elements.Add(obj);
            }
            ArrayList elementC3 = new ArrayList();
            StringBuilder temp;
            //l2与自身进行连接
            for (int i = 0; i < elements.Count; i++)
            {
                string s1 = elements[i].ToString();
                temp = new StringBuilder(s1);
                for (int j = i + 1; j < elements.Count; j++)
                {
                    string s2 = elements[j].ToString();
                    temp.Append(s2);
                    //若相连接的两个元素中含有相同子集，则去掉重复的元素后进行连接
                    if (IsExistRepeatElement(temp,out temp))
                    {
                        int count = 0;
                        if (!c3.ContainsKey(temp.ToString()))
                        {                          
                            ArrayList temps = new ArrayList();
                            for (int k = 0; k< temp.Length; k++)
                            {
                                temps.Add(temp[k]);
                            }
                            //剪枝操作
                            string add = "true";
                            ArrayList al = new ArrayList();  
                            for (int b = 0; b < temps.Count; b++)
                            {
                                al =(ArrayList) temps.Clone();
                                al.RemoveAt(b);
                                if (!IsInL2(al))
                                {
                                    add = "false";
                                } 
                             }
                            //在数据库的事物中循环比较，计算c3元素对应的value值
                            foreach (ArrayList T in List)
                            {
                                if (T.Contains(temps[0].ToString()) && T.Contains(temps[1].ToString()) && T.Contains(temps[2].ToString()))
                                {
                                    count++;
                                }
                            } 
                            //结果存入字典中
                            if (add == "true")
                            {
                                c3.Add(temp.ToString(), count); 
                            }
                            count = 0;
                        }
                    }
                    temp = new StringBuilder(s1);
                }
            }
        }

        /// <summary>
        /// 判断一个字符串是否在L2中
        /// </summary>
        public static bool IsInL2(ArrayList al)
        {
            for (int i = 0; i < l2.Count; i++)
            {
                if (l2.ContainsKey(al[0].ToString() + al[1].ToString()) || l2.ContainsKey(al[1].ToString() + al[0].ToString()))
                {
                    return true;
                }
                else 
                    return false;
            }
            return false;
        }

        public static void Init()
        {
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("数据库有5个事物。设min_sup=60%，min_conf=80%");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("TID                        购买的商品");
            Console.WriteLine("T100                     {M, O, N, K, E, Y}");
            Console.WriteLine("T200                     {D, O, N, K, E, Y}");
            Console.WriteLine("T300                     {M, A, K, E}");
            Console.WriteLine("T400                     {M, U, C, K, Y}");
            Console.WriteLine("T500                     {C, O, O, K, I, E}");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("使用Apriori算法找出所有的频繁项集");
            Console.WriteLine("----------------------------------------------------");
        }

        public static void Print(Dictionary<string, int> Dic)
        {
            foreach (string s in Dic.Keys)
            {
                Console.WriteLine(s + "   " + Dic[s]);
            }
        }
    }
}
