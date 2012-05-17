using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

namespace FilterPipeLines
{
    public class Program
    {

        public void getKeyMethod()
        {
            string inputFile = @"D:\Logs\ifh.txt";
            string ifhFile = @"D:\Logs\ifhFile.txt";

            var input = new StreamReader(inputFile);
            var output = new StreamWriter(ifhFile);
            string line = string.Empty;
            string code = string.Empty;

            // get the key method
            while ((line = input.ReadLine()) != null)
            {
                int start = line.IndexOf("\"") + 1;
                code = line.Substring(start, (line.LastIndexOf("\"") - start));

                output.WriteLine(code);


            }
            input.Close();
            output.Close();
        }

        public void retrieveMethodsFromSQLFile()
        {
            string inputFile = @"D:\Logs\Methods.sql";
            string MethodsFile = @"D:\Logs\MethodsFile.txt";

            var input = new StreamReader(inputFile);
            var output = new StreamWriter(MethodsFile);
            string line = string.Empty;
            string method = string.Empty;

            while ((line = input.ReadLine()) != null)
            {
                int start = 0;

                if ((start = line.IndexOf("values")) > 0)
                {
                    // get method id
                    var intermediateStep = line.Substring(start, line.Length - start);
                    int id_start = intermediateStep.IndexOf("(") + 1;
                    int id_end = intermediateStep.IndexOf(",");
                    var method_id = intermediateStep.Substring(id_start, (id_end - id_start));


                    // get method name;
                    int name_start = intermediateStep.IndexOf("\'") + 1;
                    int name_end = intermediateStep.LastIndexOf("\'");
                    var method_name = intermediateStep.Substring(name_start, (name_end - name_start));

                    output.WriteLine(method_id + " " + method_name);
                }


            }
            input.Close();
            output.Close();

        }

        public void MethodAndID()
        {
            string MethodsFile = @"D:\Logs\MethodsFile.txt";

            var input = new StreamReader(MethodsFile);

            string line = string.Empty;

            Dictionary<string, int> methods = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            while ((line = input.ReadLine()) != null)
            {
                string[] data = line.Split();
                //Console.WriteLine(data[0] + " & " + data[1].ToLower());

                methods.Add((data[1]), Convert.ToInt32(data[0]));
            }
            input.Close();

            string ifhfile = @"D:\Logs\ifhFile.txt";
            string ifhfileID = @"D:\Logs\ifhFileWithID.txt";
            var methodsInput = new StreamReader(ifhfile);
            var methodsOuput = new StreamWriter(ifhfileID);
            string method = string.Empty;
            //var keys = methods.Keys;

            while ((method = methodsInput.ReadLine()) != null)
            {
                string temp = "ifh" + method;
                if (methods.ContainsKey(temp))
                {
                    Console.WriteLine(methods[temp]);
                    methodsOuput.WriteLine(method + " " + methods[temp]);
                }

            }
            methodsInput.Close();
            methodsOuput.Close();

        }

        public void OptimalPipeFile()
        {
            // ID is key
            var methodsMap = new Dictionary<string, string>();

            string ifhfileID = @"D:\Logs\ifhFileWithID.txt";
            var methodsInput = new StreamReader(ifhfileID);

            string method = string.Empty;

            while ((method = methodsInput.ReadLine()) != null)
            {
                string[] data = method.Split();
                // id is key
                methodsMap[data[1]] = data[0];
            }
            methodsInput.Close();

            string PipeFile = @"D:\Logs\output3.txt";
            string OptimalPipeFile = @"D:\Logs\OptimalPipe.txt";
            
            var input = new StreamReader(PipeFile);
            var output1 = new StreamWriter(OptimalPipeFile);
            
            string line = string.Empty;
            var PipeLines = new Dictionary<string, long>();

            while ((line = input.ReadLine()) != null)
            {
                string[] data = line.Split();
                foreach (var num in data)
                {
                    if (methodsMap.ContainsKey(num))
                    {
                        if (PipeLines.ContainsKey(line))
                        {
                            Console.WriteLine("See: " + PipeLines[line]);
                            PipeLines[line]++;
                        }
                        else
                        {
                            PipeLines.Add(line, 1); 
                            output1.WriteLine(line);
                        }
                        
                        break;
                    }
                }
            }
            input.Close();
            output1.Close();
            /*
            string OptimalPipeFreq = @"D:\Logs\OptimalPipeFreq.txt";
            var output2 = new StreamWriter(OptimalPipeFreq);
            foreach (var pipe in PipeLines)
            {
                output2.WriteLine(pipe.Value);
            }
            output2.Close();
             * */
        }
        
        private bool ContainsCluster(Dictionary<string, string> clusterMap, string method)
        {

            return true;
        }
        
        // the same id, e.g. 1 1 1 1 == 1x4
        public static bool checkCluster1(Dictionary<string, int> clusterMap, Dictionary<string, string> methodsMap, string[] methods, ref int index, StringBuilder result)
        {
            int N = 1;
            var cluster = new StringBuilder();
            cluster.Append(methods[index] + " "); // add the first important id
            string current = methods[index];
            var size = methods.Length;
            int i; // point to the final stop position
            for (i = index+1; i <size; i++)
            {
                string next = methods[i];
                bool exist = methodsMap.ContainsKey(next);
                if (!exist && N == 1)
                    return false;
                else if (!exist)
                    break;
                // 2 2 2 2 2 == 2x5
                if (next.Equals(current))
                {
                    N++;
                    cluster.Append(next + " ");
                    continue;
                }
                else
                    break;
            }

            
            if (N > 1)
            {
                string temp = "[" + current + "]x" + N;
                result.Append(temp + " "); // 2 2 2 2 2 == [2]x5
                string clusterString = cluster.ToString();

                if (clusterMap.ContainsKey(temp))
                    clusterMap[temp]++;
                else
                    clusterMap.Add(temp, 1);

                if (!methodsMap.ContainsKey(temp))
                    methodsMap.Add(temp, clusterString.Remove(clusterString.Length - 1, 1));

                index = i - 1; // point the last number of cluster
            }
            else
                return false;

            
            return true;
            
        }

        public static bool checkCluster3(Dictionary<string, int> clusterMap, Dictionary<string, string> methodsMap, string[] methods, ref int index, StringBuilder result)
        {
            var cluster = new StringBuilder();
            cluster.Append(methods[index] + " "); // add the first important id
            string current = methods[index];
            var size = methods.Length;

            string next = methods[index+1];
            if (!methodsMap.ContainsKey(next))
                return false;
            cluster.Append(methods[index+1] + " "); // add the second important id
            int N = 1;
            int length = 2;  // record the number of ids in the cluster
       
            bool found = false;
            int start;
            for ( start = index + length ; start < size; start++)
            {
                var substring = new StringBuilder();
                int j;
                // find out next N numbers, see if they match cluster
                for (j = 0; j < length; j++)
                {
                }
            }

            return true;
        }
        /*
        private static string existedClusters(Dictionary<string, int> clusterMap, Dictionary<string, string> methodsMap, string[] methods, int index)
        {


        }
         * */

        private static void addCluster(Dictionary<string, int> diffclusterMap, string cluster)
        {
            if (diffclusterMap.ContainsKey(cluster))
            {
                diffclusterMap[cluster]++;
            }
            else
            {
                diffclusterMap.Add(cluster, 1);
            }

        }

        public static void MatchClusters(Dictionary<string, int> diffclusterMap, Dictionary<string, string> methodsMap, Dictionary<string, int> clusterMap, ref string[] modifiedPipeLine)
        {
            var newClusterMap = new Dictionary<string, int>();
            bool modified = false;
            foreach (var data in diffclusterMap.Keys)
            {
                int size = modifiedPipeLine.Length;
                string cluster = string.Empty;
                if (data.Substring(0, 1) == "[") //contains log entry
                {
                    //codecount++;
                    cluster = data.Substring(1, (data.LastIndexOf("]") - 1));
                    //cluster.Remove(cluster.Length - 1, 1);
                }
                var temp = new StringBuilder();
                string[] methods = cluster.Split('_');
                if (methods.Length == 1)
                    continue;

                int numOfmethods = methods.Length;

                bool match = true;
                int N = 0;
                
                for (int i = 0; i < size; i++)
                {
                    match = true;
                    // done, rest part is less than the size of cluster
                    if ((size - i) < numOfmethods)
                    {
                        if (N > 1)
                        {
                            string Acluster = "[" + cluster + "]x" + N;
                            temp.Append(Acluster + " ");
                            addCluster(newClusterMap, Acluster);
                            modified = true;
                            N = 0;
                        }
                        temp.Append(modifiedPipeLine[i] + " ");
                        match = false;
                        continue;    //   check the rest string!!!!!!!!!!!!!!!
                    }
                    int j = 0;
                    for (; j < numOfmethods; j++)
                    {
                        // after first several strings match a cluster, after that doesn't, so wrap it
                        if (!methods[j].Equals(modifiedPipeLine[i + j]))
                        {
                            if (N >= 1)
                            {
                                string Acluster = "[" + cluster + "]x" + N;
                                temp.Append(Acluster + " ");
                                addCluster(newClusterMap, Acluster);
                                modified = true;
                                N = 0;
                                /*
                                 * TODO how to stop it and restart matching from current position?
                                 * */
                                //temp.Append(modifiedPipeLine[i] + " ");
                            }
                            match = false;
                            break;
                        }

                    }

                    if (match)
                    {
                        N++;
                        //temp.Append(data + " ");
                        i = i + j-1;
                    }
                    else
                    {
                        temp.Append(modifiedPipeLine[i] + " ");
                    }
                }
                if (N >= 1)
                {
                    string Acluster = "[" + cluster + "]x" + N;
                    temp.Append(Acluster + " ");
                    addCluster(newClusterMap, Acluster);
                    modified = true;
                }
                temp.Remove(temp.Length - 1, 1);
                modifiedPipeLine = temp.ToString().Split();

            }
            if (modified)
            {
                appendClusterMap(diffclusterMap, newClusterMap);
                appendMethodsMap(methodsMap, clusterMap, newClusterMap);
            }
        }

        private static void appendClusterMap(Dictionary<string, int> diffclusterMap, Dictionary<string, int> newClusterMap)
        {
            foreach (var data in newClusterMap)
            {
                if (diffclusterMap.ContainsKey(data.Key))
                    diffclusterMap[data.Key] += data.Value;
                else
                    diffclusterMap.Add(data.Key, data.Value);
            }

        }
        public static bool checkCluster2(Dictionary<string, int> diffClusterMap, Dictionary<string, int> clusterMap, Dictionary<string, int> uniqueCluster, Dictionary<string, string> methodsMap, string[] methods, ref int index, StringBuilder result)
        {
            var cluster = new StringBuilder();
            cluster.Append(methods[index] + "_"); // add the first important id
            //string current = methods[index];

            //var allclusters = diffClusterMap.Keys; // get all current found keys
            // if current method is the last one then return
            if (index + 1 > methods.Length - 1)
                return false;

            string next = methods[index+1];
            if (!methodsMap.ContainsKey(next))
                return false;
            cluster.Append(methods[index+1] + "_"); // add the second important id

            //existedClusters(clusterMap, methods, index);

            int N = 1;
            int length = 2;  // record the number of ids in the cluster
       
            bool found = false;
            var size = methods.Length;
            int start;
            for ( start = index + length ; start < size; start++)
            {
                // length of rest part is less than cluster
                if (length > methods.Length - start)
                    break;

                var substring = new StringBuilder();
                int j;
                // find out next N numbers, see if they match cluster
                for (j = 0; j < length; j++)
                {
                    if (start + j == size) // over pipeline size
                        break;

                    substring.Append(methods[start + j] + "_");
                }

                if (cluster.Equals(substring))
                {
                    N++;
                    found = true;
                    start += length-1;
                }
                else
                {
                    if (found)
                    {
                        break;
                    }
                    
                    // if next length long string doesn't match cluster, then cluster add the start number
                    if (methodsMap.ContainsKey(methods[start]))
                    {
                        cluster.Append(methods[start] + "_");
                        length++; // cluster size increase
                        
                    }
                    else
                        break;
                }

            }

            if (N >= 1)
            {
                string clusterString = cluster.Remove(cluster.Length-1, 1).ToString();
                string temp = "[" + clusterString + "]x" + N;
                result.Append(temp + " "); // [12_3]x2 4 67   ???     " " ???
                //result.Append(substring.ToString());
                buildCluster(N, temp, diffClusterMap, clusterMap, uniqueCluster, methodsMap);
                index = start-1;
            }
            else
            {
                /*
                size = methods.Length - index;
                for (int i = 0; i < size; i++)
                {
                    if (methodsMap.ContainsKey(methods[index + i]))
                    {
                        if (checkCluster2(clusterMap, methodsMap, modifiedPipeLine, ref i, secondStep_result))
                            continue;
                    }

                    secondStep_result.Append(modifiedPipeLine[i] + " ");  // current method id is not important

                }
                 * */ 
            }
            
            
            return true;
               
                
        }

        private static void buildCluster(int N, String temp, Dictionary<string, int> diffclusterMap, Dictionary<string, int> clusterMap, Dictionary<string, int> uniqueCluster, Dictionary<string, string> methodsMap)
        {

            if (diffclusterMap.ContainsKey(temp))
                diffclusterMap[temp]++;
            else
                diffclusterMap.Add(temp, 1);

            if (clusterMap.ContainsKey(temp))
                clusterMap[temp]++;
            else
                clusterMap.Add(temp, 1);

            string clusterString = string.Empty;
            if (temp.Substring(0, 1) == "[") //contains log entry
            {
                clusterString = temp.Substring(1, (temp.LastIndexOf("]") - 1));
            }
            string origCluster = clusterString.Replace('_', ' ');

            if (!methodsMap.ContainsKey(temp))
            {
                methodsMap.Add(temp, origCluster);
            }

            if (!uniqueCluster.ContainsKey(origCluster))
            {
                uniqueCluster.Add(origCluster, N);
            }
            else
            {
                uniqueCluster[origCluster] += N;
            }

        }
        private static void CheckModifiedPipeLine(Dictionary<string, int> diffclusterMap, Dictionary<string, string> methodsMap, ref string[] modifiedPipeLine, int index, StringBuilder secondStep_result)
        {
            /*
                 * [TODO] check if clusters in the map are existed in this pipeline
                 * */
            if (diffclusterMap.Count != 0)
            {
                //MatchClusters(diffclusterMap, ref modifiedPipeLine);
                //modifiedPipeLine = secondStep_result.ToString().Split();

            }

            int size = modifiedPipeLine.Length;
            string[] str = new string[size-index];

            if (size-1 == 1)
                return;

            for (int j = 0; j < size - index; j++)
            {
                str[j] = modifiedPipeLine[index + j];
            }
            var middle = new StringBuilder();
            CheckModifiedPipeLine(diffclusterMap, methodsMap, ref str, index--, middle);
            //var result = new StringBuilder();
            size = str.Length;
            // iterate the pipeline
            for (int i = 0; i < size; i++)
            {
                if (methodsMap.ContainsKey(str[i]))
                {
                    /*
                     * TODO add clusterMap as paramenter
                     * 
                    if (checkCluster2(diffclusterMap, methodsMap, str, ref i, middle))
                        continue;
                     * */
                }

                secondStep_result.Append(modifiedPipeLine[i] + " ");  // current method id is not important

            }

            secondStep_result.Append(middle.ToString());
            
        }

        public static bool OneMethodCluster(Dictionary<string, int> clusterMap, Dictionary<string, string> methodsMap, string[] methods, ref int index, StringBuilder result)
        {
            int N = 1;
            var cluster = new StringBuilder();
            cluster.Append(methods[index] + " "); // add the first important id
            string current = methods[index];
            var size = methods.Length;
            int i; // point to the final stop position
            for (i = index + 1; i < size; i++)
            {
                string next = methods[i];
                bool exist = methodsMap.ContainsKey(next);
                if (!exist && N == 1)
                    return false;
                else if (!exist)
                    break;
                // 2 2 2 2 2 == 2x5
                if (next.Equals(current))
                {
                    N++;
                    cluster.Append(next + " ");
                    continue;
                }
                else
                    break;
            }


            if (N > 1)
            {
                string temp = "[" + current + "]x" + N;
                result.Append(temp + " "); // 2 2 2 2 2 == [2]x5
                string clusterString = cluster.ToString();

                if (clusterMap.ContainsKey(temp))
                    clusterMap[temp]++;
                else
                    clusterMap.Add(temp, 1);
                
                if (!methodsMap.ContainsKey(temp))
                    methodsMap.Add(temp, clusterString.Remove(clusterString.Length - 1, 1));
                
                index = i - 1; // point the last number of cluster
            }
            else
                return false;


            return true;

        }

        public static void SameMethodIds(Dictionary<string, string> methodsMap, Dictionary<string, int> clusterMap)
        {
            // @"C:\Users\Song\Dropbox\Intern\Logs\test.txt";
            string OptimalFile = @"D:\Logs\OptimalPipe.txt";
            var PipeInput = new StreamReader(OptimalFile);
            // @"C:\Users\Song\Dropbox\Intern\Logs\testClusterPipe.txt"; 
            string cluster = @"D:\Logs\clusteredPipe1.txt";
            var output = new StreamWriter(cluster);

            string pipeLine = string.Empty;
            //var clusterMap = new Dictionary<string, int>();
            var result = new StringBuilder();
            while ((pipeLine = PipeInput.ReadLine()) != null)
            {
                string[] origPipeLine = pipeLine.Split();
                
                var size = origPipeLine.Length;
                //Console.WriteLine("Size: " + size);
                // firstly, check all continuous equivalent numbers, e.g. 1 2 2 2 3 4 5 3 2 2 == 1 2x3 4 5 3 2x2
                for (int i = 0; i < size; i++)
                {
                    if (methodsMap.ContainsKey(origPipeLine[i]))
                    {
                        if (OneMethodCluster(clusterMap, methodsMap, origPipeLine, ref i, result))
                            continue;
                    }

                    result.Append(origPipeLine[i] + " ");  // current method id is not important
                }
                result.Remove(result.Length - 2, 2);                  // important, see if an white space at last
                output.WriteLine(result.ToString());
                result.Clear();
            }

            PipeInput.Close();
            output.Close();

        }
        // print in the order
        private static void reOrderPipeLine(Dictionary<string, int> clusterList, StreamWriter output)
        {
            foreach (var data in clusterList.OrderBy(key => key.Value))
            {
                output.WriteLine(data.Key);

            }
        }
        // all id are the key in the pipeline
        public static void allKeys(Dictionary<string, string> methodsMap, Dictionary<string, int> clusterMap)
        {
            string OptimalFile = @"D:\Logs\clusteredPipe1.txt"; // testClusterPipe.txt";
            var PipeInput = new StreamReader(OptimalFile);
            string cluster = @"D:\Logs\purePipe.txt";  // testPurePipe.txt";
            var output1 = new StreamWriter(cluster);
            string pipel = @"D:\Logs\mixPipe.txt";  // testMixPipe.txt"; 
            var output2 = new StreamWriter(pipel);

            string pipeLine = string.Empty;
            //var pure_result = new StringBuilder();
            //var result = new StringBuilder();
            var pure_Cluster = new Dictionary<string, int>();
            var mix_Cluster = new Dictionary<string, int>();
            int counter = 1;
            while ((pipeLine = PipeInput.ReadLine()) != null)
            {
                string[] origPipeLine = pipeLine.Split();

                var size = origPipeLine.Length;

                var pure = true;
                for (int i = 0; i < size; i++)
                {
                    if (!methodsMap.ContainsKey(origPipeLine[i]) && !clusterMap.ContainsKey(origPipeLine[i]))
                    {
                        pure = false;
                        break;
                    }
                }

                if (pure)
                {
                    pure_Cluster.Add(pipeLine, size);
                    //output1.WriteLine(pipeLine);
                }
                else
                {
                    mix_Cluster.Add(pipeLine, size);
                    //output2.WriteLine(pipeLine);
                }
                counter++;
            }

            reOrderPipeLine(pure_Cluster, output1);
            reOrderPipeLine(mix_Cluster, output2);

            PipeInput.Close();
            output1.Close();
            output2.Close();
        }

        public static bool TrainingClusterSet(Dictionary<string, int> clusterMap, string[] methods, ref int index, StringBuilder result)
        {
            var cluster = new StringBuilder();
            cluster.Append(methods[index] + "_"); // add the first important id
            string current = methods[index];

            var allclusters = clusterMap.Keys; // get all current found keys
            // if current method is the last one then return
            
            if (index + 1 > methods.Length - 1)
                return false;
            
            string next = methods[index + 1];
            
            cluster.Append(methods[index + 1] + "_"); // add the second important id

            //existedClusters(clusterMap, methods, index);

            int N = 1;
            int length = 2;  // record the number of ids in the cluster

            bool found = false;
            var size = methods.Length;
            int start;
            for (start = index + length; start < size; start++)
            {
                // length of rest part is less than cluster
                if (length > methods.Length - start)
                {
                    /*
                    for (int i = start; i < methods.Length; i++)
                    {
                        cluster.Append(methods[i] + "_");
                    }
                     * */
                    break;
                }

                var substring = new StringBuilder();
                int j;
                // find out next N numbers, see if they match cluster
                for (j = 0; j < length; j++)
                {
                    if (start + j == size) // over pipeline size
                        break;

                    substring.Append(methods[start + j] + "_");
                }

                if (cluster.Equals(substring))
                {
                    N++;
                    found = true;
                    start += length - 1;
                }
                else
                {
                    if (found)
                    {
                        break;
                    }
                    
                    cluster.Append(methods[start] + "_");
                    length++; // cluster size increase
                    

                }

            }

            if (N > 1)
            {
                string clusterString = cluster.Remove(cluster.Length - 1, 1).ToString();
                string temp = "[" + clusterString + "]x" + N;
                result.Append(temp + " "); // [12_3]x2 4 67 
                //result.Append(substring.ToString());

                if (clusterMap.ContainsKey(temp))
                    clusterMap[temp]++;
                else
                    clusterMap.Add(temp, 1);
                /*
                if (!methodsMap.ContainsKey(temp))
                {
                    string origCluster = clusterString.Replace('_', ' ');
                    methodsMap.Add(temp, origCluster);

                }
                 */
                index = start - 1;
            }
            else
                return false;
            
            return true;

        }
        
        private static void searchClusters(Dictionary<string, int> clusterMap, ref string[] cluster)
        {
            var temp = new StringBuilder();
            if (cluster.Length < 4) // 2, or 3
            {
                foreach (var data in cluster)
                {
                    temp.Append(data + "_");
                }
                temp.Remove(temp.Length-1, 1);
                string Acluster = "[" + temp + "]x1";
                addCluster(clusterMap, Acluster);
                /*
                for(int i=0; i+1<cluster.Length; i++)
                {
                    string temp = cluster[i] + "_" + cluster[i+1];
                    string Acluster = "[" + temp + "]x1";
               
                    addCluster(clusterMap, Acluster);
                }
                 * */
            }
            else
            {
                /*
                 * TODO
                 * */
                //MatchClusters(clusterMap, ref cluster); 
            }

        }

        private static void addCluster(Dictionary<string, int> clusterMap, ref string[] cluster, StreamWriter output)
        {
            var result = new StringBuilder();
            bool found = false;
            while (!found)
            {
                /*
                 * TODO
                 * */
                if(cluster.Length>3)
                    //MatchClusters(clusterMap, ref cluster); 
                //searchClusters(clusterMap, ref cluster);
                
                // iterate the pipeline
                for (int i = 0; i < cluster.Length; i++)
                {
                    if (!found && TrainingClusterSet(clusterMap, cluster, ref i, result))
                    {

                        found = true;

                        continue;
                    }

                    result.Append(cluster[i] + "_");  // current method id is not important

                }
                result.Remove(result.Length - 1, 1);
                if (found)
                {
                    cluster = result.ToString().Split('_');
                    result.Clear();
                    found = false;
                }
                else
                {
                    
                    break;
                }
            }
            //result.Remove(result.Length - 1, 1);
            string temp = "[" + result.ToString() + "]x1";
            addCluster(clusterMap, temp);
            
            output.WriteLine(temp);
        }
        
        public static void buildClusterMap(Dictionary<string, int> clusterMap)
        {
            string clusterFile = @"D:\Logs\testPurePipe.txt"; //purePipe.txt";
            var inputCluster = new StreamReader(clusterFile);
            string outputFile = @"D:\Logs\TestOutput.txt";
            var output = new StreamWriter(outputFile);

            string pipeLine = string.Empty;

            while ((pipeLine = inputCluster.ReadLine()) != null)
            {
                string[] methods = pipeLine.Split();
                int size = methods.Length;
                var clusters = new StringBuilder();

                if (size == 1)
                    continue;


                addCluster(clusterMap, ref methods, output);
                /*
                for (int i = 0; i < size; i++)
                {
                    clusters.Append(methods[i]);
                    for (int j = i + 1; j < size; j++)
                    {
                        clusters.Append(methods[j]);
                        addCluster(clusterMap, clusters);
                    }
                }
                */

            }
        }

        public static void learnSameMethodIds(Dictionary<string, string> methodsMap, Dictionary<string, int> clusterMap)
        {

            string OptimalFile = @"D:\Logs\OptimalPipe.txt";//test.txt";
            var PipeInput = new StreamReader(OptimalFile);
            string cluster = @"D:\Logs\clusteredPipe2.txt";
            var output = new StreamWriter(cluster);

            string pipeLine = string.Empty;
            
            // the second cluster map for different id combinations
            //var diffclusterMap = new Dictionary<string, int>();

            while ((pipeLine = PipeInput.ReadLine()) != null)
            {
                string[] origPipeLine = pipeLine.Split();
                var firstStep_result = new StringBuilder();
                var size = origPipeLine.Length;
                //Console.WriteLine("Size: " + size);
                // firstly, check all continuous equivalent numbers, e.g. 1 2 2 2 3 4 5 3 2 2 == 1 2x3 4 5 3 2x2
                for (int i = 0; i < size; i++)
                {
                    if (methodsMap.ContainsKey(origPipeLine[i]))
                    {
                        if (checkCluster1(clusterMap, methodsMap, origPipeLine, ref i, firstStep_result))
                            continue;
                    }

                    firstStep_result.Append(origPipeLine[i] + " ");  // current method id is not important
                }
                firstStep_result.Remove(firstStep_result.Length - 2, 2);
                output.WriteLine(firstStep_result);
            }
            PipeInput.Close();
            output.Close();
        }

        public static void splitIntoPureAndMix(Dictionary<string, string> methodsMap)
        {
            string OptimalFile = @"D:\Logs\clusteredPipe2.txt"; // testClusterPipe.txt";
            var PipeInput = new StreamReader(OptimalFile);
            string cluster = @"D:\Logs\purePipe2.txt";  // testPurePipe.txt";
            var output1 = new StreamWriter(cluster);
            string pipel = @"D:\Logs\mixPipe2.txt";  // testMixPipe.txt"; 
            var output2 = new StreamWriter(pipel);

            string pipeLine = string.Empty;
            
            var pure_Cluster = new Dictionary<string, int>();
            var mix_Cluster = new Dictionary<string, int>();
            int counter = 1;
            while ((pipeLine = PipeInput.ReadLine()) != null)
            {
                //pipeLine.Remove(pipeLine.Length - 2, 2);
                string[] origPipeLine = pipeLine.Split();

                var size = origPipeLine.Length;

                var pure = true;
                for (int i = 0; i < size; i++)
                {
                    if (!methodsMap.ContainsKey(origPipeLine[i]))
                    {
                        pure = false;
                        break;
                    }
                }

                if (pure)
                {
                    pure_Cluster.Add(pipeLine, size);
                    //output1.WriteLine(pipeLine);
                }
                else
                {
                    mix_Cluster.Add(pipeLine, size);
                    //output2.WriteLine(pipeLine);
                }
                counter++;
            }

            reOrderPipeLine(pure_Cluster, output1);
            reOrderPipeLine(mix_Cluster, output2);

            PipeInput.Close();
            output1.Close();
            output2.Close();
        }

        private static void appendMethodsMap(Dictionary<string, string> methodsMap, Dictionary<string, int> clusterMap, Dictionary<string, int> newClusterMap )
        {
            foreach (var cluster in newClusterMap)
            {
                if (clusterMap.ContainsKey(cluster.Key))
                    clusterMap[cluster.Key]++;
                else
                    clusterMap.Add(cluster.Key, 1);

                if (!methodsMap.ContainsKey(cluster.Key))
                {
                    string clusterString = string.Empty;
                    if (cluster.Key.Substring(0, 1) == "[") //contains log entry
                    {
                        clusterString = cluster.Key.Substring(1, (cluster.Key.LastIndexOf("]") - 1));
                    }
                    string origCluster = clusterString.Replace('_', ' ');
                    methodsMap.Add(cluster.Key, origCluster);

                }
            } 

        }

        private static void appendUniqueCluster(Dictionary<string, int> uniqueCluster, string cluster, int N)
        {
            if (uniqueCluster.ContainsKey(cluster))
                uniqueCluster[cluster] += N;
            else
                uniqueCluster.Add(cluster, N);

        }

        private static string MatchCluster1(Dictionary<string, int> diffclusterMap, Dictionary<string, int> uniqueCluster, Dictionary<string, string> methodsMap, Dictionary<string, int> clusterMap, string[] modifiedPipeLine)
        {
            int size = modifiedPipeLine.Length;
           
            var newClusterMap = new Dictionary<string, int>();
            bool modified = false;
            var  result = new StringBuilder();
            int i = 0;
            foreach (var cluster in uniqueCluster.Keys)
            {
                /*
                string cluster = string.Empty;
                if (data.Substring(0, 1) == "[") //contains log entry
                {
                    //codecount++;
                    cluster = data.Substring(1, (data.LastIndexOf("]") - 1));
                    //cluster.Remove(cluster.Length - 1, 1);
                }
                
                string[] methods = cluster.Split('_');
                 * */
                string[] methods = cluster.Split();
                if (methods.Length == 1)  // e.g. [25]x3, check the next cluster, like [23_45]x2
                    continue;

                int numOfmethods = methods.Length;

                if (numOfmethods > size)
                    continue;

                bool match = true;
                int N = 0;
                result.Clear();
                //['2', '3', '5', '7', '12'] size == 5
                i = 0;
                for (; i < size; i++)
                {
                    match = true;
                    // done, rest part is less than the size of cluster
                    if ((size - i) < numOfmethods)
                    {
                        if (N > 1)
                        {
                            string temp = cluster;
                            string Acluster = "[" + temp.Replace(' ', '_') + "]x" + N;
                            result.Append(Acluster + " ");
                            addCluster(newClusterMap, Acluster);
                            appendClusterMap(diffclusterMap, newClusterMap);
                            appendMethodsMap(methodsMap, clusterMap, newClusterMap);
                            appendUniqueCluster(uniqueCluster, cluster, N);
                            modified = true;
                            N = 0;

                            if (size - i == 1)
                            {
                                result.Append(modifiedPipeLine[i]+" ");
                            }
                            else if (size - i > 1)
                            {
                                string[] rest = new string[size - i];

                                for (int j = 0; j < size - i; j++)
                                {
                                    rest[j] = modifiedPipeLine[i + j];
                                }
                                // recursion, check rest part if matching
                                result.Append(MatchCluster1(diffclusterMap, uniqueCluster, methodsMap, clusterMap, rest));
                            }
                            return result.ToString();

                        }
                        /*
                        for (int j = i; j < size; j++ )
                            result.Append(modifiedPipeLine[j] + " "); // the rest number
                        */
                        match = false;
                        break;
                        //   stop checking the rest string, check the next cluster !!!!!!!!!!!!!!!
                    }
                    int k = 0;
                    for (; k < numOfmethods; k++)
                    {
                        // after first several strings match a cluster, after that doesn't, so wrap it
                        if (!methods[k].Equals(modifiedPipeLine[i + k]))
                        {
                            if (N > 0)
                            {
                                string temp = cluster;
                                string Acluster = "[" + temp.Replace(' ', '_') + "]x" + N;
                                result.Append(Acluster + " ");
                                addCluster(newClusterMap, Acluster);
                                appendClusterMap(diffclusterMap, newClusterMap);
                                appendMethodsMap(methodsMap, clusterMap, newClusterMap);
                                appendUniqueCluster(uniqueCluster, cluster, N);
                                modified = true;
                                N = 0;

                                string[] rest = new string[size - i];

                                for (int j = 0; j < size - i; j++)
                                {
                                    rest[j] = modifiedPipeLine[i + j];
                                }
                                // recursion, check rest part if matching
                                result.Append(MatchCluster1(diffclusterMap, uniqueCluster, methodsMap, clusterMap, rest));

                                return result.ToString();
                            }
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        N++;
                        //temp.Append(data + " ");
                        i = i + k - 1;
                        continue;
                    }

                    result.Append(modifiedPipeLine[i] + " ");

                }
                // all match the cluster  12 23 12 23 12 23 == [12 23]x3
                if (N > 0)
                {
                    string temp = cluster;
                    string Acluster = "[" + temp.Replace(' ', '_') + "]x" + N;
                    result.Append(Acluster + " ");
                    addCluster(diffclusterMap, Acluster);
                    appendUniqueCluster(uniqueCluster, cluster, N);
                    modified = true;
                    if (size - i == 1)
                    {
                        result.Append(modifiedPipeLine[i]+ " ");
                    }
                    else if (size - i > 1)
                    {
                        string[] rest = new string[size - i];

                        for (int j = 0; j < size - i; j++)
                        {
                            rest[j] = modifiedPipeLine[i + j];
                        }
                        // recursion, check rest part if matching
                        result.Append(MatchCluster1(diffclusterMap, uniqueCluster, methodsMap, clusterMap, rest));
                    }

                    // TODO one more recursion! 
                    //      [301_[55]x2]x1
                    //      [[301_[55]x2]x1_94]x1
                    //      [[301_[55]x2]x1_94_[301_[55]x2]x1_94_301]x1
                    
                    return result.ToString();
                    
                }

            }

            if (modified)
            {
                appendClusterMap(diffclusterMap, newClusterMap);
                appendMethodsMap(methodsMap, clusterMap, newClusterMap);
            }

            for (int x = i; x < size; x++)
            { 
                result.Append(modifiedPipeLine[x] + " ");
            }

            // TODO one more recursion! 
            //      [301_[55]x2]x1
            //      [[301_[55]x2]x1_94]x1
            //      [[301_[55]x2]x1_94_[301_[55]x2]x1_94_301]x1
            return result.ToString();
        }
        /*
        private static string checkCluster(Dictionary<string, int> diffclusterMap, Dictionary<string, int> clusterMap, Dictionary<string, string> methodsMap, string[] origPipeLine)
        {
        
        }
        */
        private static String searchClusters(Dictionary<string, string> methodsMap, Dictionary<string, int> clusterMap, Dictionary<string, int> diffclusterMap, Dictionary<string, int> uniqueCluster, StringBuilder pipeLine)
        {
            string[] origPipeLine = pipeLine.ToString().Split();
            var size = origPipeLine.Length;

            if (size == 1) // [23]x3
                return ""; //pipeLine.ToString();

            string cluster = string.Empty;
            if (size == 2) // 23 14
            {
                string temp = pipeLine.Replace(' ', '_').ToString();
                cluster = "[" + temp + "]x1";
                buildCluster(1, cluster, diffclusterMap, clusterMap, uniqueCluster, methodsMap);
                return cluster;//temp.ToString;   
            }

            //CheckModifiedPipeLine(diffclusterMap, methodsMap, ref modifiedPipeLine, 0, secondStep_result);
            string result = string.Empty;
            // match any cluster in the map appears at pipeline
            if (uniqueCluster.Count != 0 || origPipeLine.Length != 1)
            {
                string temp = MatchCluster1(diffclusterMap, uniqueCluster, methodsMap, clusterMap, origPipeLine);
                result = temp.Remove(temp.Length - 1, 1);
                string modify_result = result;
                // e.g. 301 [55]x2 94
                if (modify_result.Contains(" "))
                {
                    cluster = "[" + modify_result.Replace(' ', '_') + "]x1";
                    buildCluster(1, cluster, diffclusterMap, clusterMap, uniqueCluster, methodsMap);
                }
                else
                {// if result is [12_23_44]x3, don't add [...]x1
                    // TODO  what if 12 23 44 12 23 44 12 23 44 == [12_23_44]x3
                    int start = modify_result.LastIndexOf("x") + 1;
                    int end = modify_result.Length - start;
                    string N = modify_result.Substring(start, end);
                    //buildCluster(Convert.ToInt32(N), modify_result, diffclusterMap, clusterMap, uniqueCluster, methodsMap);
                    
                }
            }

            return result;        

        }

        private static string finalSearchClusters(Dictionary<string, string> methodsMap, Dictionary<string, int> clusterMap, Dictionary<string, int> diffclusterMap, Dictionary<string, int> uniqueCluster, string pipeLine)
        {
            string[] origPipeLine = pipeLine.Split();
            var size = origPipeLine.Length;
            if (size < 3) // [23]x4, or [24_[25]x4]x1, or [37_[21]x4]x1 42, 
                return pipeLine;

            var result = new StringBuilder();
            var temp = new StringBuilder();
            int counter = 0;   // the number a single number
            foreach (var data in origPipeLine)
            {
                if (data.Contains('x'))
                {
                    if (counter == 0)  // [23_34]x5
                    {
                        result.Append(data + " ");
                        
                    }
                    else if (counter == 1)  // 2 [23_3]x4
                    {
                        result.Append(temp.ToString() + data + " ");
                        temp.Clear();
                        counter = 0;
                        
                    }
                    else // 2 3 [23_3]x2
                    {
                        string cluster = temp.ToString().Remove(temp.ToString().Length-1, 1);
                        string diffCluster = "[" + cluster.Replace(' ', '_') + "]x1";
                        buildCluster(1, diffCluster, diffclusterMap, clusterMap, uniqueCluster, methodsMap);
                        result.Append(diffCluster + " " + data + " ");
                        counter = 0;
                        temp.Clear();
                        
                    }
                }
                else
                {
                    temp.Append(data + " ");
                    counter++;
                }
            }

            if (counter == 0)
            {
                result.Remove(result.Length-1, 1);
            }
            else if (counter == 1)  // 2 [23_3]x4, or 2 [23_3]x4 23
            {
                result.Append(temp.ToString().Remove(temp.ToString().Length - 1, 1));
            }
            else if (counter > 1) // 2 3 [23_3]x2 23 4
            {
                string cluster = temp.ToString().Remove(temp.ToString().Length - 1, 1);
                string diffCluster = "[" + cluster.Replace(' ', '_') + "]x1";
                buildCluster(1, diffCluster, diffclusterMap, clusterMap, uniqueCluster, methodsMap);
                result.Append(diffCluster);

            }

            return result.ToString();
        }

        public static void learnFromPure(Dictionary<string, string> methodsMap, Dictionary<string, int> clusterMap)
        {
            string OptimalFile = @"D:\Logs\test2.txt";  //purePipe2.txt"; //
            var PipeInput = new StreamReader(OptimalFile);
            string cluster = @"D:\Logs\testResult.txt"; //pureClusters.txt"; // final pure cluster result  //testOutput.txt"; 
            var output = new StreamWriter(cluster);

            string diffClusters = @"D:\Logs\diffClusters.txt";
            var output2 = new StreamWriter(diffClusters);
            string uniqueMap = @"D:\Logs\uniqueClusters.txt";
            var output3 = new StreamWriter(uniqueMap);

            string pure_result = @"D:\Logs\pure_results.txt";  // the final one: 244 [23_12
            var output4 = new StreamWriter(pure_result);

            string pipeLine = string.Empty;
            string result = string.Empty;
            //var clusterMap = new Dictionary<string, int>();
            // the second cluster map for different id combinations
            var diffclusterMap = new Dictionary<string, int>(); // [[2]x3, 23], [[23_12]x2, 4]
            var uniqueCluster = new Dictionary<string, int>();  // ["23 12", 4]
            int counter = 0;
            while ((pipeLine = PipeInput.ReadLine()) != null)
            {
                //string[] origPipeLine = pipeLine.Split();
                //var size = origPipeLine.Length;

                Console.WriteLine("line " + counter++); 
                var input = new StringBuilder();
                input.Append(pipeLine);
                result = searchClusters(methodsMap, clusterMap, diffclusterMap, uniqueCluster, input);
                if (result.Equals(""))
                {
                    // [23]x3
                    output.WriteLine(input.ToString());
                }
                else
                {
                    output.WriteLine(result);
                    //string final = finalSearchClusters(methodsMap, clusterMap, diffclusterMap, uniqueCluster, result);
                    //output4.WriteLine(final);
                }
            }

            foreach(var data in diffclusterMap)
            {
                output2.WriteLine(data.Key + " " + data.Value);
            }

            foreach(var data in uniqueCluster)
            {
                output3.WriteLine("[" + data.Key + "] " + data.Value);
            }
              

            PipeInput.Close();
            output.Close();
            output2.Close();
            output3.Close();
            output4.Close();

        }

        public static void BuildMethodsMap(Dictionary<string, string> methodsMap, string directory)
        {
            var methodsInput = new StreamReader(directory);

            string method = string.Empty;

            while ((method = methodsInput.ReadLine()) != null)
            {
                string[] data = method.Split();
                // id is key
                methodsMap[data[1]] = data[0];
            }
            methodsInput.Close();

        }

        public static void Main(string[] args)
        {
            // ID is key
            var methodsMap = new Dictionary<string, string>();
            var clusterMap = new Dictionary<string, int>();


            string ifhfileID = @"D:\Logs\ifhFileWithID.txt";
            BuildMethodsMap(methodsMap, ifhfileID);
            /*
            var clusterMap = new Dictionary<string, int>();
            SameMethodIds(methodsMap, clusterMap);
            allKeys(methodsMap, clusterMap);
            buildClusterMap(clusterMap);
            */
            
            learnSameMethodIds(methodsMap, clusterMap);
            splitIntoPureAndMix(methodsMap);
            learnFromPure(methodsMap, clusterMap);

            string clusterfile = @"D:\Logs\clusterMap.txt";
            var clusters = new StreamWriter(clusterfile);
            foreach (var data in clusterMap)
            {
                clusters.WriteLine(data.Key + " " + data.Value);
            }
            clusters.Close();


            /*
             * TODO read pureClusters.txt, new clusterMap for new found cluster
             * split pipe by " " , [12_34]x3 23 45 67 --- so [23_45_67]x1 is add into new clusterMap
             * */
            
            
            /*
            string OptimalFile = @"D:\Logs\OptimalPipe.txt";//test.txt";
            var PipeInput = new StreamReader(OptimalFile);
            string cluster = @"D:\Logs\testOutput.txt";//clusterPipe10.txt";
            var output = new StreamWriter(cluster);

            string pipeLine = string.Empty;
            var clusterMap = new Dictionary<string, int>();
            // the second cluster map for different id combinations
            var diffclusterMap = new Dictionary<string, int>();

            while ((pipeLine = PipeInput.ReadLine()) != null)
            {
                string[] origPipeLine = pipeLine.Split();
                var firstStep_result = new StringBuilder();
                var size = origPipeLine.Length;
                //Console.WriteLine("Size: " + size);
                // firstly, check all continuous equivalent numbers, e.g. 1 2 2 2 3 4 5 3 2 2 == 1 2x3 4 5 3 2x2
                for (int i = 0; i < size; i++)
                {
                    if (methodsMap.ContainsKey(origPipeLine[i]))
                    {
                        if (checkCluster1(clusterMap, methodsMap, origPipeLine, ref i, firstStep_result))
                            continue;     
                    }

                    firstStep_result.Append(origPipeLine[i] + " ");  // current method id is not important
                }
                firstStep_result.Remove(firstStep_result.Length - 2, 2);
                output.WriteLine(firstStep_result.ToString());
                // firstStep_result.Remove(firstStep_result.Length - 1, 1);
                // secondly, the same pipeline, I am going to check the cluster with different randomly numbers
                // e.g. 1 2x3 4 5 3 2x2 2x3 4 5 3 2x2 2x3 4 5 3 2x2 == 1 [2x3 4 5 3 2x2]x3
                string[] modifiedPipeLine = firstStep_result.ToString().Split();
                size = modifiedPipeLine.Length;
                var secondStep_result = new StringBuilder();

                //Console.WriteLine("Size: " + modifiedPipeLine.Length);
                

                //CheckModifiedPipeLine(diffclusterMap, methodsMap, ref modifiedPipeLine, 0, secondStep_result);
                bool found = false;
                while (!found)
                {
                    // [TODO] check if clusters in the map are existed in this pipeline

                    if (diffclusterMap.Count != 0)
                    {
                        MatchClusters(diffclusterMap, ref modifiedPipeLine);
                        //modifiedPipeLine = secondStep_result.ToString().Split();

                    }
                    // iterate the pipeline
                    for (int i = 0; i < modifiedPipeLine.Length; i++)
                    {
                        if (methodsMap.ContainsKey(modifiedPipeLine[i]))
                        {
                            if (!found && checkCluster2(diffclusterMap, methodsMap, modifiedPipeLine, ref i, secondStep_result))
                            {
                               
                                found = true;
                                
                                continue;
                            }

                        }

                        secondStep_result.Append(modifiedPipeLine[i] + " ");  // current method id is not important

                    }
                    secondStep_result.Remove(secondStep_result.Length - 1, 1);
                    if (found){
                        modifiedPipeLine = secondStep_result.ToString().Split();
                        secondStep_result.Clear();
                        found = false;
                    }
                    else
                        break;
                }
                secondStep_result.Remove(secondStep_result.Length - 1, 1);
                output.WriteLine(secondStep_result.ToString());
                
                
            }

            PipeInput.Close();
            output.Close();
            */
            /*
             * [TODO] combine both cluster map
             * */

            
            /*
            // test cluster map
            foreach (var data in clusterMap)
            {
                Console.WriteLine(data.Key + " ~ " + data.Value);
            }
            */

            /*
            // testing methodsMap
            foreach (var data in methodsMap)
            {
                Console.WriteLine(data.Key + " == " + data.Value);
            }
             * */
        }
    }


    public class Cluster
    {
        public Cluster(string c) { cluster = c; }
        public string cluster { get; set; }

        public bool CompareTo(string value)
        {

            return true;
        }
    }

    /*
    public class myDictionary<TKey, TValue>:Dictionary<TKey, TValue>
    {
        public myDictionary(){}

        public bool ContainsKey(TKey key);

    }
     * */
}
