using AutoMapper;
using mediQueue.API.Model.DTO;
using mediQueue.API.Model.Entity;
using static mediQueue.API.Model.DTO.AppointmentDTO;

namespace mediQueue.API.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {


            //====================
            // Patient Mapper
            //====================
            CreateMap<Patient, PatientDTO.Response>().ReverseMap();
            CreateMap<PatientDTO.Create, Patient>();
            CreateMap<PatientDTO.Update, Patient>();


            //====================
            // User Mapper
            //====================
            CreateMap<User, UserDTO.Response>()
                // 1. Map the relationships explicitly
                .ForMember(dest => dest.DoctorProfile, opt => opt.MapFrom(src => src.DoctorProfile))
                .ForMember(dest => dest.ReceptionistProfile, opt => opt.MapFrom(src => src.ReceptionistProfile))
                .ReverseMap();

            CreateMap<UserDTO.Create, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            //====================
            // Doctor Mapper
            //====================
            CreateMap<DoctorDTO.Create, Doctor>();
            CreateMap<DoctorDTO.Update, Doctor>();

            CreateMap<Doctor, DoctorDTO.Response>();


            //====================
            // Receptionist Mapper
            //====================
            CreateMap<ReceptionistDTO.Create, Receptionist>();
            CreateMap<Receptionist, ReceptionistDTO.Response>();


            //====================
            // APPOINTMENT Mapper
            //====================
            CreateMap<AppointmentDTO.Create, Appointment>();
            CreateMap<Appointment, AppointmentDTO.Response>();
            CreateMap<Appointment, AppointmentWithPatientDto>()
                .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient.Id))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.Name))
                .ForMember(dest => dest.PatientPhone, opt => opt.MapFrom(src => src.Patient.PhoneNumber))
                .ForMember(dest => dest.BloodGroup, opt => opt.MapFrom(src => src.Patient.BloodGroup));


            //====================
            // Invoice Mapper
            //====================
            CreateMap<InvoiceDTO.Create, Invoice>();
            CreateMap<Invoice, InvoiceDTO.Response>();
        }
    }
}
