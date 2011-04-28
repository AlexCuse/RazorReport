namespace RazorReport
{
    public interface IEngine<T> {
        void Compile (string preparedTemplate, string name);
        string Run (T model, string name);
        string Parse (string template, T model);
    }
}