# Transferencias 🚀

Este es un proyecto de aplicación móvil para la gestión de **transferencias de stock entre depósitos**. Su propósito es funcionar como una herramienta para el sistema ERP Calipso de Escorial, permitiendo a los usuarios mover inventario de manera eficiente.

La aplicación está construida con **.NET MAUI** y **.NET 8**, lo que le permite funcionar en múltiples plataformas.

---

## Características Principales ✨

* **Integración con ERP Calipso**: Diseñado para interactuar directamente con el sistema ERP de la empresa.
* **Transferencia de Stock**: Permite realizar transferencias de inventario entre diferentes depósitos.
* **Escaneo Versátil 🤳**: La aplicación tiene la capacidad de leer códigos de barra y QR. Es compatible con:
    * La cámara del dispositivo móvil.
    * Lectores láser integrados (probado con Zebra TC21).
    * Lectores externos (conectados vía Bluetooth o USB).
* **Dependencia de API**: Para funcionar, la aplicación se comunica con una API dedicada.
* **Creación de Transferencias**: Inicia nuevas solicitudes de transferencia de stock.
* **Seguimiento de Estado**: Consulta el estado actual de las transferencias enviadas.
* **Validación de Datos**: Se integra con el ERP para validar depósitos, productos y existencias.
* **Interfaz Sencilla**: Diseñada para ser intuitiva y fácil de usar por el personal de logística.

---

## Requisitos y Dependencias 🛠️

Para compilar y ejecutar esta aplicación, necesitarás:

* **Visual Studio 2022**: Con la carga de trabajo de **Desarrollo de interfaz de usuario de .NET Multi-platform App UI** (MAUI) instalada.
* **.NET 8 SDK**.
* **TransferenciasApiCore**: Debes tener el servicio de la API funcionando y configurado. Puedes encontrar el repositorio aquí: [TransferenciasApiCore](https://github.com/ESCORIAL-SAIC/TransferenciasApiCore).
* **EtiquetasTransferencias**: El software para la impresión de etiquetas que luego son escaneadas por esta aplicación. Puedes encontrar el repositorio aquí: [EtiquetasTransferencias](https://github.com/ESCORIAL-SAIC/EtiquetasTransferencias).

---

## Instalación y Configuración ⚙️

1.  **Clonar el repositorio**:
    ```bash
    git clone [https://github.com/ESCORIAL-SAIC/Transferencias.git](https://github.com/ESCORIAL-SAIC/Transferencias.git)
    ```
2.  **Abrir en Visual Studio**:
    Abre el archivo de la solución (`Transferencias.sln`) en Visual Studio.
3.  **Restaurar paquetes**:
    Asegúrate de restaurar los paquetes de NuGet para resolver las dependencias.
4.  **Compilar**:
    Compila la solución para generar el paquete de la aplicación para la plataforma deseada (actualmente solo hay soporte para Android).

---

## Uso 📲

Una vez que la aplicación esté instalada y compilada, el primer paso es configurarla:

1.  **Configuración de la API**: Dentro de la aplicación, busca la pantalla de configuraciones. Desde allí, podrás ingresar la URL de la API a la cual debe conectarse.
2.  **Selección de depósitos**: En la misma pantalla de configuraciones, podrás seleccionar el depósito de origen y destino para la transferencia.
3.  **Escanear los productos** a transferir utilizando la cámara de tu dispositivo o un lector de código de barras externo. Los códigos a escanear deben haber sido generados e impresos previamente con el software **EtiquetasTransferencias**.
4.  **Procesar la transferencia** a través de la interfaz de la aplicación, que se comunicará con la API para registrar los movimientos en el ERP.

---

## Contribución 🤝

Si deseas contribuir a este proyecto, por favor, sigue el flujo de trabajo estándar de GitHub:

1.  Haz un "fork" del repositorio.
2.  Crea una nueva rama (`git checkout -b feature/nueva-funcionalidad`).
3.  Haz "commit" de tus cambios.
4.  Sube tus cambios a tu "fork".
5.  Crea un "Pull Request".

---

## Contacto 📧

Si tienes alguna pregunta o sugerencia, no dudes en contactar a los contribuidores principales.

* **maurinicolas**
