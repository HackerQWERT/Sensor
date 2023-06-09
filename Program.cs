SensorDataSimulator simulator = new();
await simulator.ConnectToServer();

class SensorDataSimulator
{
    readonly HubConnection connection;
    static readonly string ip = "127.0.0.1";
    static readonly string port = "5147";
    readonly string url = $"http://{ip}:{port}/SensorHub";

    public SensorDataSimulator()
    {
        connection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console() // 将日志输出到控制台
            .WriteTo.File("log.txt") // 将日志写入到文件
            .CreateLogger();

        connection.Closed += async (error) =>
        {
            // 连接关闭时尝试重新连接
            await ConnectToServer();
        };
    }

    public async Task ConnectToServer()
    {
        try
        {
            await connection.StartAsync();
            Log.Information("已连接到服务器");

            // 模拟传感器数据并发送到服务器
            while (true)
            {
                // 休眠一段时间
                // 生成模拟传感器数据
                double temperature = GenerateRandomTemperature();
                double pressure = GenerateRandomPressure();
                double vibration = GenerateRandomVibration();

                // 构建传感器数据对象
                SensorData data = new()
                {
                    Time = DateTime.Now,
                    Temperature = temperature,
                    Pressure = pressure,
                    Vibration = vibration
                };

                // 将传感器数据发送到服务器
                await connection.SendAsync("ReceiveSensorData", data);

                await Task.Delay(1000);

                // 休眠一段时间
            }
        }
        catch (Exception ex)
        {
            Log.Information("无法连接到服务器: {0}", ex.Message);
        }
    }

    private double GenerateRandomTemperature()
    {
        // 生成随机温度值
        Random random = new();
        double minTemperature = -40;
        double maxTemperature = 100;
        double temperature = random.NextDouble() * (maxTemperature - minTemperature) + minTemperature;
        return Math.Round(temperature, 2);
    }

    private double GenerateRandomPressure()
    {
        // 生成随机压力值
        Random random = new();
        double minPressure = 0;
        double maxPressure = 100;
        double pressure = random.NextDouble() * (maxPressure - minPressure) + minPressure;
        return Math.Round(pressure, 2);
    }

    private double GenerateRandomVibration()
    {
        // 生成随机振动值
        Random random = new();
        double minVibration = 0;
        double maxVibration = 1.0;
        double vibration = random.NextDouble() * (maxVibration - minVibration) + minVibration;
        return Math.Round(vibration, 2);
    }
}

// 传感器数据对象
class SensorData
{

    public int SensorId { get; set; }
    public DateTime Time { get; set; }
    public double Temperature { get; set; }
    public double Pressure { get; set; }
    public double Vibration { get; set; }
}


