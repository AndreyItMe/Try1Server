using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Try1Server;
using Try1Server.Massage;
using static System.Collections.Specialized.BitVector32;
//http://localhost:8080/post/e.txt/q

//убрать /post
//добавить /message - имя ресурса с котоырм я работаю
//остальное можно убрать в параметры POST-запроса

class TextFileServer
{
    HttpListener listener;
    string FILE_STORAGE_PATH = Path.GetFullPath("C:\\Users\\andrey\\Desktop\\4sem\\КСиС\\курсач\\Try1Server\\Try1Server");
    public List<TextMassage> ListMassage = new List<TextMassage>(); //массив всех сообщений
    public List<User> ListUser = new List<User>(); //массив всех пользователей
#pragma warning disable SYSLIB0011
    //BinaryFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011

    public TextFileServer(string prefix)
    {
        listener = new HttpListener();
        listener.Prefixes.Add(prefix);
    }

    public void Start()
    {
        listener.Start();
        Console.WriteLine("Server started...");
        Listen();
    }
    public async void Listen()
    {
        while (true)
        {
            try
            {
                HttpListenerContext context = await listener.GetContextAsync();
                ProcessRequest(context);

                string someString = "201";
                byte[] bytes = Encoding.ASCII.GetBytes(someString);
                //context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                context.Response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }

    public void DeSerializationAllContacts()
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented

        };
        string path = Path.Combine(FILE_STORAGE_PATH, "message.txt");
        //var path = Path.Combine(Environment.CurrentDirectory, "contactJSON.txt");
        var json = File.ReadAllText(path);

        ListMassage = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TextMassage>>(json, settings);
    }

    public void DeSerializationAllUsersJSON()
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented

        };
/*
        string jsonOut = Newtonsoft.Json.JsonConvert.SerializeObject(ListUser, settings);
        string pathOut = Path.Combine(Environment.CurrentDirectory, "user.txt");
        File.WriteAllText(pathOut, jsonOut);
*/

        //string path = Path.Combine(FILE_STORAGE_PATH, "user.txt");
        var path = Path.Combine(Environment.CurrentDirectory, "user.txt");
        var json = File.ReadAllText(path);

        ListUser = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(json, settings);
    }

    public void SerializationAllContactsJSON()
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented

        };
/*
        var json = File.ReadAllText(path);
        TextMassage? currency = Newtonsoft.Json.JsonConvert.DeserializeObject<TextMassage>(json, settings);
        ListMassage.Add(currency);

        currency.SenderName = "Ivan";
        currency.Text = "Hello";
        ListMassage.Add(currency);
*/
        //сериализация 
        string jsonOut = Newtonsoft.Json.JsonConvert.SerializeObject(ListMassage, settings);
        var path = Path.Combine(FILE_STORAGE_PATH, "message.txt");
        File.WriteAllText(path, jsonOut);
/*
            //сериализация 
            var jsonOut = Newtonsoft.Json.JsonConvert.SerializeObject(currency, settings);
            var pathOut = Path.Combine(Environment.CurrentDirectory, "contactOut.txt");
            File.WriteAllText(pathOut, jsonOut);
*/
    }

    public void SerializationAllContacts()
    {
        //TextMassage Massage;
        /*
                using (FileStream fs = new FileStream("SerializeBIN.bin", FileMode.OpenOrCreate))
                {
                    for(int i = 0; i < ListMassage.Count; i++)
                    {
                        TextMassage Massage = (TextMassage)ListMassage[i];
                        //formatter.Serialize(fs, Massage);
                        fs.Flush();
                    }
                    fs.Close();
                    Console.WriteLine("DeSerializationAllContacts complete");
                }
        */
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Newtonsoft.Json.Formatting.Indented
        };

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(ListMassage, settings);
    }

    public bool loginCorrect(string  username, string password)
    {
        foreach (User user in ListUser)
        {
            if(username == user.UserName & password == user.UserPassword) 
            { 
                return true;
            }
        }
        return false;
    }

    public void ProcessRequest(HttpListenerContext context)
    {
        //string method = context.Request.HttpMethod;
        string requestedUrl = context.Request.Url.LocalPath.Trim('/');
        string[] urlParts = requestedUrl.Split('/');
        //string action = urlParts[0].ToLower();
        string action = context.Request.HttpMethod;
        //string target = urlParts.Length >= 1 ? requestedUrl.Substring(action.Length + 1) : "";
        //string target = requestedUrl;
        //target - место куда запизывается информация
        string target = "contact.txt";
        //Console.WriteLine(context.ToString());
        //rawurl
        //Console.WriteLine(context.Request.ToString());
        Console.WriteLine(context.Request.RawUrl);
        //добавить проверку на то правильный ли пользователь или нет, передавая его контакты с каждым запросом
        //string username = "";
        //string password = "";
        var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
        var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
        var username = credentials[0];
        var password = credentials[1];
        if (loginCorrect(username, password) )
        {
            switch (action)
            {  
                case "GET":
                    //button2
                    ServeFile(context, target);
                    break;
                /*
                            case "PUT": //put
                                SaveFile(context, target);
                                break;
                */
                case "POST":
                    //button1
                    if (urlParts[0] == "message")//
                    {
                        if (context.Request.Url.Segments.Length == 2)
                        {
                           AppendToFile(context, target);
                           //SaveFile(context, target);
                        }
                        if (context.Request.Url.Segments.Length == 4)
                        {
                            AppendToFile(context, target);
                        }
                    }
                    if (urlParts[0] == "user")//
                    {
                        if (context.Request.Url.Segments.Length == 2)
                        {
                            //AppendToFile(context, target);
                            //SaveFile(context, target);
/*
                            var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
                            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                            var username = credentials[0];
                            var password = credentials[1];
                            if(loginCorrect(username, password))
*/
                            
                                //смысл добавлять пользователя если он уже есть в базе?!
                                //ListUser.Add(new User(username, password));
                                //пускаем пользователя дальше из меню LOGIN 
                                //отправить ответ что все хорошо
                                //https://stackoverflow.com/questions/18165616/rest-http-status-code-for-success-and-thus-you-no-longer-have-access#:~:text=200%20is%20the%20appropriate%20response%2C%20because%20it%20indicates,not%20mean%20a%20subsequent%20GET%20can%27t%20return%20403.
                                //тут говорится что код 201 для успешного создания
                                //HttpListenerContext
                           
                                context.Response.StatusCode = ((int)HttpStatusCode.Created);
                            
/*
                            else
                            {
                                //отправить ответ что все плохо
                                ListUser.Add(new User(username, password));
                                context.Response.StatusCode = ((int)HttpStatusCode.Unauthorized);
                            }
*/
                        }
                    }
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    context.Response.Close();
                    break;
            }
        }
        else
        {
            //отправить ответ что все плохо
            ListUser.Add(new User(username, password));
            context.Response.StatusCode = ((int)HttpStatusCode.Unauthorized);
        }
    }

    public void AppendToFile(HttpListenerContext context, string fileName)
    {
        string filePath = Path.Combine(FILE_STORAGE_PATH, fileName); //!!!
        try
        {
            using (StreamWriter writer = File.AppendText(filePath))
            {
                using (StreamReader reader = new StreamReader(context.Request.InputStream)) //вот тут находятся входные значения
                {
                    TextMassage currency= new TextMassage();
                    string content = reader.ReadToEnd();
                    currency.Text = content;
                    //currency.Time = DateTime.Now - DateTime.UtcNow;
                    currency.Time = DateTime.Now.TimeOfDay;

                    var authHeader1 = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
                    var credentialBytes1 = Convert.FromBase64String(authHeader1.Parameter);
                    var credentials1 = Encoding.UTF8.GetString(credentialBytes1).Split(':', 2);
                    var username = credentials1[0];
                    var password = credentials1[1];
                    currency.SenderName = username;

                    var authHeader2 = AuthenticationHeaderValue.Parse(context.Request.Headers["ReceiverName"]);
                    var ReceiverName = authHeader2.ToString();
                    currency.ReceiverName = ReceiverName;
                    

                    ListMassage.Add(currency);
                }
            }
            //добавил новое сообщение в массив сообщщений ListMassage и теперь надо снова его сериализовать
            SerializationAllContactsJSON();
                    //content == "helloQAZA"
                    //writer.Write(content);
                
            
            context.Response.StatusCode = (int)HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            Console.WriteLine("Error: " + ex.Message);
        }
        context.Response.Close();
    }

    public void ServeFile(HttpListenerContext context, string fileName)
    {
        string filePath = Path.Combine(FILE_STORAGE_PATH, fileName);
        try
        {
            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath); //content содежит json контакт
                byte[] buffer = Encoding.UTF8.GetBytes(content);

                //TextMassage bufer = Newtonsoft.Json.JsonConvert.DeserializeObject<TextMassage>(content); //<TextMassage> -> List<TextMassage>
                List<TextMassage>? bufer = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TextMassage>>(content);
                //ListMassage.Add(bufer);

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "text/plain";
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            Console.WriteLine("Error: " + ex.Message);
        }
        context.Response.Close();
    }

    public void Stop()
    {
        listener.Stop();
    }
}



class Program
{
    static void Main(string[] args)
    {
        const string SERVER_URL = "http://localhost:8080/";
        //const string SERVER_URL = "http://192.168.1.54:8080/";
        //192.168.1.54

        TextFileServer server = new TextFileServer(SERVER_URL);

        //сериалазация, которая будет считывать все извесные сообщения
        //server.SerializationAllContacts();
        //server.DeSerializationAllContacts();
        server.DeSerializationAllContacts();
/*
        User user = new User();
        user.UserName = "andrey";
        user.UserPassword = "qwerty";
        server.ListUser.Add(user);
*/
        server.DeSerializationAllUsersJSON();
        server.Start();

        Console.WriteLine("Press any key to stop the server...");
        Console.ReadKey();

        server.Stop();
    }
}