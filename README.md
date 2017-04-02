# PaenkoDB_CSharpLib
CSharp Library that implements communication with the PaenkoDB REST interface

To include this library in your project download it with Nuget.
https://www.nuget.org/packages/Paenko.PaenkoDb/1.0.0

Example usage:

~~~csharp
static void Main(string[] args)
        {
            Node node = new Node(IPAddress.Parse("207.154.216.94"), 3000); // Create New Node object to work with
            node.Login(new Authentication("ich", "pw")); // Authenticate with username and password
            UuidManager.Load("ids.json"); // Load the locally saved Uuids
            var logs = node.GetLogs(); // Get all the logs
            
            logs.ForEach(l => Console.WriteLine($"log: {l}")); // And display them

            if (UuidManager.LookForId("mainlog") == null) // If "mainlog" isn't already in the UuidManager then add it
            {
                UuidManager.Add(logs[0], "mainlog", UuidObject.UuidType.Log);
            }

            TestKlasse tk = new TestKlasse(); // Create Random class (it has 2 properties: int a = 5; int b = 10)
            Document x = Document.FromObject<TestKlasse>(tk); // Turn the class into a document that can be saved on the server

            if (UuidManager.LookForId("TestClassFile") == null) //  If "TestClassFile" isn't already in the UuidManager then post it to the server
            {
                node.PostDocument(x, UuidManager.LookForId("mainlog"), "TestClassFile");
            }

            Console.WriteLine(" ");
            UuidManager.LookAll(UuidObject.UuidType.All).ForEach(u => Console.WriteLine($"{u.Id} {u.Description}")); // Show all ids in the Manager

            var resp = node.GetDocument(UuidManager.LookForId("mainlog"), UuidManager.LookForId("TestClassFile")); // Get the document we just sent
            var ret = resp.ToObject<TestKlasse>(); // Deserialize it
            Console.WriteLine($"a: {ret.a} b: {ret.b}"); // Display its properties

            UuidManager.SaveIds("ids.json"); // Save the Ids in UuidManager to ids.json
            Console.ReadLine();
        }
~~~