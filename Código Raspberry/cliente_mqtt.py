import serial
import json
import paho.mqtt.client as mqtt

# Configura el objeto Serial
ser = serial.Serial('/dev/ttyUSB0', 9600, timeout=1)

# Configura la conexión MQTT
mqtt_broker_host = "localhost"  # Cambia al host real si es diferente
mqtt_topic = "arduino/voltaje"  # Cambia al tema MQTT deseado

# Inicialización del cliente MQTT
mqtt_client = mqtt.Client()

try:
    mqtt_client.connect(mqtt_broker_host, 1883, 60)
    print("Conectado al broker MQTT")

    while True:
        # Lee una línea desde el puerto serie
        line = ser.readline().decode("utf-8").strip()

        # Imprime el valor leído en la terminal
        print("Valor leído por Arduino:", line)

        # Extrae la información relevante y forma un diccionario JSON
        try:
            raw_value = line.split(", ")
            value_data = {}
            for entry in raw_value:
                key, value = entry.split(": ")
                value_data[key.strip()] = value.strip()

            # Publica el JSON en el tema MQTT
            mqtt_client.publish(mqtt_topic, json.dumps(value_data))
            print("Dato publicado en MQTT:", json.dumps(value_data))

        except Exception as e:
            print(f"Error al procesar los datos: {e}")

except KeyboardInterrupt:
    # Cierra el puerto serie al salir del programa
    ser.close()
    print("Programa terminado.")