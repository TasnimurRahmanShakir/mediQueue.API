using AutoMapper;
using mediQueue.API.Model.DTO;
using mediQueue.API.Model.Entity;

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
            CreateMap<AppointmentDTO.Create, Appointment>()
                .ForMember(dest => dest.Scedule, opt => opt.MapFrom(src => src.Schedule));
            CreateMap<Appointment, AppointmentDTO.Response>()
                .ForMember(dest => dest.Schedule, opt => opt.MapFrom(src => src.Scedule))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.User.Name))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.Name));


            //====================
            // Invoice Mapper
            //====================
            CreateMap<InvoiceDTO.Create, Invoice>();
            CreateMap<Invoice, InvoiceDTO.Response>();
        }
    }
}
