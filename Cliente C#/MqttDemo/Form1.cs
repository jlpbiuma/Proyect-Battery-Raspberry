using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Newtonsoft.Json; // LIBRERÍA NECESARIA PARA LEER EL FORMATO JSON

namespace MqttDemo
{
    public partial class Form1 : Form
    {
        private MqttClient mqttClient;
        private Label labelRawValues;
        private Label labelVoltage;

        public Form1()
        {
            InitializeComponent();

            // Inicializar el cliente MQTT
            InitializeMqttClient();

            // Inicializar controles NumericUpDown
            InitializeNumericUpDownControls(); // Esta función es SOLO para posicionar algunos elementos en la ventana
        }

        private void InitializeMqttClient() // FUNCIÓN PARA INICIALIZAR EL CLIENTE MQTT
        {
            // Reemplazar con la dirección IP o nombre de host de tu broker MQTT
            string brokerAddress = "192.168.0.17";
            int brokerPort = 1883;

            // Crear una nueva instancia de MqttClient
            mqttClient = new MqttClient(brokerAddress, brokerPort, false, null, null, MqttSslProtocols.None);

            // Adjuntar un controlador de eventos para manejar mensajes MQTT entrantes
            mqttClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            Console.WriteLine($"Created MQTT client for {brokerAddress}:{brokerPort}");

            try
            {
                // Conectar al broker MQTT
                mqttClient.Connect(Guid.NewGuid().ToString());

                // Suscribirse a un topic MQTT
                string topic = "arduino/voltaje"; // TIENE QUE SER EL MISMO TOPIC QUE EN LA RASPBERRY
                mqttClient.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

                listBox1.Items.Add($"Connected to MQTT broker, subscribed to topic '{topic}'");
            }
            catch (Exception ex)
            {
                // Manejar error de conexión
                listBox1.Items.Add($"Error connecting to MQTT broker: {ex.Message}");
            }
        }

        private void InitializeNumericUpDownControls()
        {
            // Crear y configurar los controles NumericUpDown
            numericUpDown1 = new NumericUpDown();
            numericUpDown1.Location = new Point(150, 50);  // Adjust the coordinates as needed
            numericUpDown1.Parent = this;
            numericUpDown1.Size = new System.Drawing.Size(70, 22);
            numericUpDown1.ReadOnly = true; // Make it read-only
            numericUpDown1.Minimum = 0;
            numericUpDown1.Maximum = 1024;

            numericUpDown2 = new NumericUpDown();
            numericUpDown2.Location = new Point(150, 80);  // Adjust the coordinates as needed
            numericUpDown2.Parent = this;
            numericUpDown2.Size = new System.Drawing.Size(70, 22);
            numericUpDown2.ReadOnly = true; // Make it read-only
            numericUpDown2.DecimalPlaces = 2; // Set the number of decimal places
            numericUpDown2.Minimum = 0;
            numericUpDown2.Maximum = 15;

            // Crear y configurar los controles Label
            labelRawValues = new Label();
            labelRawValues.Text = "Raw Values";
            labelRawValues.Size = new System.Drawing.Size(80, 22);
            labelRawValues.Location = new Point(numericUpDown1.Left - labelRawValues.Width - 10, numericUpDown1.Top + (numericUpDown1.Height - labelRawValues.Height) / 2);  // Adjust the coordinates as needed
            labelRawValues.TextAlign = ContentAlignment.MiddleLeft;
            labelRawValues.Padding = new Padding(5, 0, 0, 0); // Adjust padding as needed
            labelRawValues.Parent = this;
            labelRawValues.Visible = true;

            labelVoltage = new Label();
            labelVoltage.Text = "Voltage";
            labelVoltage.Size = new System.Drawing.Size(80, 22);
            labelVoltage.Location = new Point(numericUpDown2.Left - labelVoltage.Width - 10, numericUpDown2.Top + (numericUpDown2.Height - labelVoltage.Height) / 2);  // Adjust the coordinates as needed
            labelVoltage.TextAlign = ContentAlignment.MiddleLeft;
            labelVoltage.Padding = new Padding(5, 0, 0, 0); // Adjust padding as needed
            labelVoltage.Parent = this;  // Corrected assignment
            labelVoltage.Visible = true;
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) // FUNCIÓN PARA RECIBIR LOS MENSAJES
        {
            // Handle incoming message
            string message = Encoding.UTF8.GetString(e.Message);

            // Parse the JSON message to extract numeric values
            var values = ParseJsonMessage(message);

            // Update the UI from a different thread
            Invoke(new Action(() =>
            {
                listBox1.Items.Add($"'{e.Topic}': {message}");

                // Update the NumericUpDown controls with the latest numeric values
                UpdateNumericUpDowns(values);
            }));
        }

        private void UpdateNumericUpDowns(Dictionary<string, double> values) // ESTA FUNCIÓN ACTUALIZA LOS VALORES DE LOS NUMERICUPDOWN
        {
            // Suponiendo que el diccionario de valores contiene valores numéricos para cada clave

            // Actualizar numericUpDown1 para "Raw Value" si la clave existe
            if (values.ContainsKey("Raw Value"))
            {
                numericUpDown1.Value = (decimal)values["Raw Value"];
            }

            // Actualizar numericUpDown2 para "Voltage" si la clave existe
            if (values.ContainsKey("Voltage"))
            {
                // Escalar el valor dividiendo por 100 y establecer el valor con dos decimales
                numericUpDown2.Value = (decimal)(values["Voltage"] / 100);
            }
        }

        private Dictionary<string, double> ParseJsonMessage(string message)
        {
            // Reemplazar esto con tu lógica real de análisis JSON
            var jsonValues = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
            var numericValues = new Dictionary<string, double>();

            foreach (var key in jsonValues.Keys)
            {
                if (double.TryParse(jsonValues[key], out double numericValue))
                {
                    numericValues[key] = numericValue;
                }
            }

            return numericValues;
        }
    }
}
