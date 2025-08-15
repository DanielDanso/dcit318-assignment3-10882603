using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystemQ2
{
    // a) Generic Repository<T>
    // Constrained to class so T? works naturally for reference types.
    public class Repository<T> where T : class
    {
        private readonly List<T> items = new List<T>();

        public void Add(T item) => items.Add(item);

        public List<T> GetAll() => new List<T>(items);

        // Return first match or null
        public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);

        public bool Remove(Func<T, bool> predicate)
        {
            var idx = items.FindIndex(x => predicate(x));
            if (idx >= 0)
            {
                items.RemoveAt(idx);
                return true;
            }
            return false;
        }
    }

    // b) Patient class
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString() => $"Patient {{ Id={Id}, Name={Name}, Age={Age}, Gender={Gender} }}";
    }

    // c) Prescription class
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString() =>
            $"Prescription {{ Id={Id}, PatientId={PatientId}, Medication={MedicationName}, Date={DateIssued:d} }}";
    }

    // g) HealthSystemApp
    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new Repository<Patient>();
        private readonly Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

        public void SeedData()
        {
            // 2–3 Patients
            _patientRepo.Add(new Patient(1, "Ama Mensah", 28, "Female"));
            _patientRepo.Add(new Patient(2, "Kwame Boateng", 35, "Male"));
            _patientRepo.Add(new Patient(3, "Akosua Owusu", 42, "Female"));

            // 4–5 Prescriptions (valid PatientIds)
            _prescriptionRepo.Add(new Prescription(101, 1, "Amoxicillin 500mg", DateTime.Today.AddDays(-7)));
            _prescriptionRepo.Add(new Prescription(102, 1, "Ibuprofen 200mg", DateTime.Today.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(103, 2, "Loratadine 10mg", DateTime.Today));
            _prescriptionRepo.Add(new Prescription(104, 3, "Vitamin D3 1000IU", DateTime.Today.AddDays(-14)));
            _prescriptionRepo.Add(new Prescription(105, 2, "Paracetamol 500mg", DateTime.Today.AddDays(-1)));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap = _prescriptionRepo
                .GetAll()
                .GroupBy(p => p.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("=== Patients ===");
            foreach (var p in _patientRepo.GetAll())
                Console.WriteLine(p);
        }

        // f) GetPrescriptionsByPatientId(...)
        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.TryGetValue(patientId, out var list)
                ? new List<Prescription>(list)
                : new List<Prescription>();
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            Console.WriteLine($"\n=== Prescriptions for PatientId={id} ===");
            var list = GetPrescriptionsByPatientId(id);
            if (list.Count == 0)
            {
                Console.WriteLine("No prescriptions found.");
                return;
            }
            foreach (var rx in list)
                Console.WriteLine(rx);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();
            // pick one PatientId to display (example: 2)
            app.PrintPrescriptionsForPatient(2);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
