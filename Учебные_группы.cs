//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Abdullin_kurs
{
    using System;
    using System.Collections.Generic;
    
    public partial class Учебные_группы
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Учебные_группы()
        {
            this.Ведомость_успеваемости = new HashSet<Ведомость_успеваемости>();
            this.Студенты = new HashSet<Студенты>();
        }
    
        public int Группа_ID { get; set; }
        public int Кафедра_ID { get; set; }
        public string Название_группы { get; set; }
        public Nullable<System.DateTime> Год_поступления { get; set; }
        public Nullable<int> Курс_обучения { get; set; }
        public Nullable<int> Кол_во_студентов_в_группе { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ведомость_успеваемости> Ведомость_успеваемости { get; set; }
        public virtual Кафедры Кафедры { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Студенты> Студенты { get; set; }
    }
}
