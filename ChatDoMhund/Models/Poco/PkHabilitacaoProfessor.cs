namespace ChatDoMhund.Models.Poco
{
	public class PkHabilitacaoProfessor
	{
		public int CodigoDoProfessor { get; set; }
		public int CodigoDoCurso { get; set; }
		public string Fase { get; set; }
		public int CodigoDaMateria { get; set; }
		public string DescricaoDoCurso { get; set; }
		public string NomeDaFase { get; set; }
		public string Ano { get; set; }

		public override string ToString()
		{
			return $"Curso: {this.CodigoDoCurso}" +
			       $", Fase: {this.Fase}" +
			       $", Matéria: {this.CodigoDaMateria}" +
			       $", Professor: {this.CodigoDoProfessor}";
		}
	}
}