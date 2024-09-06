using Sharp7;
using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing; // Renk ayarları için gerekli

namespace SiemensS7DataFetch
{
    public partial class Form1 : Form
    {
        private Timer timer;
        private S7Client client;

        public Form1()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            // Timer nesnesini oluştur ve ayarla
            timer = new Timer();
            timer.Interval = 2000; // 1 saniye
            timer.Tick += new EventHandler(ReadPLCData); // Tick olayı için ReadPLCData metodunu ekle
            timer.Start(); // Timer'ı başlat

            // PLC bağlantısını kur
            client = new S7Client();
        }

        private void ReadPLCData(object sender, EventArgs e)
        {
            try
            {
                if (client.Connected == false)
                {
                    int result = client.ConnectTo("192.168.0.14", 0, 1); // IP, Rack ve Slot bilgileri
                    if (result == 0)
                    {
                        // Bağlantı başarılı olduğunda label2'nin text rengini yeşil yap
                        label3.ForeColor = Color.LimeGreen;
                    }
                    else
                    {
                        // Bağlantı başarısız olduğunda hata mesajı göster
                        MessageBox.Show($"PLC'ye bağlanma hatası! Hata kodu: {result} - {client.ErrorText(result)}",
                                        "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Bağlantı başarısızsa geri dön
                    }
                }

                int dbNumber = 26; // Okunacak Data Block numarası
                int startAddress = 6; // Başlangıç adresi
                int amountOfData = 8; // Okunacak maksimum byte sayısı

                byte[] buffer = new byte[amountOfData];
                int readResult = client.DBRead(dbNumber, startAddress, amountOfData, buffer);

                if (readResult == 0)
                {
                    // Veriyi oku ve güncelle
                    for (int i = 0; i < amountOfData; i += 2)
                    {
                        if (i + 1 < amountOfData) // Buffer dışına çıkmamak için kontrol
                        {
                            // Byte dizisini big-endian formatına çevir
                            byte[] tempBuffer = new byte[2] { buffer[i + 1], buffer[i] };
                            ushort value = BitConverter.ToUInt16(tempBuffer, 0);

                            // Eğer 0-1 adresi ise, button1'in text özelliğine yaz
                            if (i == 0)
                            {
                                button1.Text = value.ToString();
                            }
                        }
                    }
                }
                else
                {
                    label3.ForeColor = Color.Red;
                    MessageBox.Show($"Veri okuma hatası! Hata kodu: {readResult} - {client.ErrorText(readResult)}",
                                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Timer'ı durdur ve temizle
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }

            // PLC bağlantısını kes
            if (client != null && client.Connected)
            {
                client.Disconnect();
            }

            base.OnFormClosing(e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("asdf");
        }
    }
}
