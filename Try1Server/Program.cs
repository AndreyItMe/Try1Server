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

class TextFileServer
{
    HttpListener listener;
    string FILE_STORAGE_PATH = Path.GetFullPath("C:\\Users\\andrey\\Desktop\\4sem\\КСиС\\курсач\\Try1Server\\Try1Server");
    public List<TextMassage> ListMessage = new List<TextMassage>(); //массив всех сообщений
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
                context.Response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }

    public void DeSerializationAllMassage()
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented

        };
        string path = Path.Combine(FILE_STORAGE_PATH, "message.txt");
        var json = File.ReadAllText(path);

        ListMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TextMassage>>(json, settings);
    }

    public void DeSerializationAllUsers()
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented
        };

        var path = Path.Combine(Environment.CurrentDirectory, "user.txt");
        var json = File.ReadAllText(path);

        ListUser = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(json, settings);
    }

    public void SerializationAllMassages()
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented
        };

        //сериализация 
        string jsonOut = Newtonsoft.Json.JsonConvert.SerializeObject(ListMessage, settings);
        var path = Path.Combine(FILE_STORAGE_PATH, "message.txt");
        File.WriteAllText(path, jsonOut);
    }

    public void SerializationAllContacts()
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Newtonsoft.Json.Formatting.Indented
        };

        string jsonOut = Newtonsoft.Json.JsonConvert.SerializeObject(ListUser, settings);
        var path = Path.Combine(FILE_STORAGE_PATH, "message.txt");
        File.WriteAllText(path, jsonOut);
    }

    public bool loginCorrect(string  username, string password)
    {
        //прохожу массив ListUser и ищу совпадение
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
        string requestedUrl = context.Request.Url.LocalPath.Trim('/'); //тут хранится либо "message" либо "user"
        string[] urlParts = requestedUrl.Split('/');
        string action = context.Request.HttpMethod;
        //target - место куда запизывается информация
        string target = "message.txt";
        Console.WriteLine(context.Request.RawUrl);
        //добавить проверку на то правильный ли пользователь или нет, передавая его контакты с каждым запросом
        //string username = "";
        //string password = "";
        var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
        var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
        var username = credentials[0];
        var password = credentials[1];
        //последние пять строчек - вытаскиваю пароль и имя пользователя
        if (loginCorrect(username, password) )
        {
            switch (action) //action - http метод
            {  
                case "GET":
                    //button2
                    if (urlParts[0] == "messages")//
                    {
                        GetMessages(context, target, username); //делаю GET запрос на получение какой-то части сообщений на клиент пользорвателя, чтобы отображать их там
                    }
                    break;
                case "POST":
                    //button1
                    if (urlParts[0] == "message")//
                    {
                        if (context.Request.Url.Segments.Length == 2)
                        {
                           newMassage(context, target, username);
                        }
/*
                        if (context.Request.Url.Segments.Length == 4)
                        {
                            newMassage(context, target, username);
                        }
*/
                    }
                    if (urlParts[0] == "user")//
                    {
                        if (context.Request.Url.Segments.Length == 2)
                        {
                            context.Response.StatusCode = ((int)HttpStatusCode.Created);
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

    public void newMassage(HttpListenerContext context, string fileName, string senderUsername)
    {
        string filePath = Path.Combine(FILE_STORAGE_PATH, fileName); //!!!
        try
        {
            using (StreamWriter writer = File.AppendText(filePath))
            {
                using (StreamReader reader = new StreamReader(context.Request.InputStream)) //вот тут находятся входные значения
                {
                    TextMassage currentMassage = new TextMassage();
                    string messageTextJSON = reader.ReadToEnd();
                    currentMassage.fromString(messageTextJSON);
                    ListMessage.Add(currentMassage);
                }
            }
            //добавил новое сообщение в массив сообщщений ListMassage и теперь надо снова его сериализовать
            SerializationAllMassages();
            
            context.Response.StatusCode = (int)HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            Console.WriteLine("Error: " + ex.Message);
        }
        context.Response.Close();
    }

    public void GetMessages(HttpListenerContext context, string fileName, string senderUsername)
    {
        string filePath = Path.Combine(FILE_STORAGE_PATH, fileName);
        try
        {
            if (File.Exists(filePath))
            {
                string MessagesListJSON = "";

                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Formatting.Indented
                };
                //сериализация 
                //поменять ListMassage на массив 
                List<TextMassage> ListMassageForCurrentUser = new List<TextMassage>(); //массив сообщений двух пользователей

                var authHeader2 = AuthenticationHeaderValue.Parse(context.Request.Headers["AnotherContact"]);
                var currentReceiverName = authHeader2.ToString();

                foreach (TextMassage textMassage in ListMessage) //обатные сообщения проверять
                {
                    if(
                        textMassage.SenderName == senderUsername && textMassage.ReceiverName == currentReceiverName ||
                        textMassage.ReceiverName == senderUsername && textMassage.SenderName == currentReceiverName)
                    {
                        ListMassageForCurrentUser.Add(textMassage); //отбираю сообщения для конкретного пользователя
                    }
                }
                //сериализую их и отправляю клиенту
                string jsonOut = Newtonsoft.Json.JsonConvert.SerializeObject(ListMassageForCurrentUser, settings);

                byte[] buffer = Encoding.UTF8.GetBytes(jsonOut);

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

        TextFileServer server = new TextFileServer(SERVER_URL);

        server.DeSerializationAllMassage();
        server.DeSerializationAllUsers();
        server.Start();

        Console.WriteLine("Press any key to stop the server...");
        Console.ReadKey();

        server.Stop();
    }
}