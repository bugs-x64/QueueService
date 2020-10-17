namespace QueueServiceAPI.Models
{
    /// <summary>
    /// Дто записи в очередь.
    /// </summary>
    public class QueueRecordDto
    {
        /// <summary>
        /// Номер записи
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ФИО клиента
        /// </summary>
        public string Fio { get; set; }

        /// <summary>
        /// Конкурирующая очередь
        /// </summary>
        public bool Competing { get; set; }
    }
}
