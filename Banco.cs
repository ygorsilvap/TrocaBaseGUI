using System;
using System.Collections.ObjectModel;

public class BancoClass
{
	public BancoClass()
	{

        public void SelectBase(ObservableCollection<Banco> db)
        {
            foreach (var item in db)
            {
                if (item.Name == conexao)
                {
                    item.Name += " (Banco Selecionado)";
                }
            }
        }

        public void UnselectBase(ObservableCollection<Banco> db)
        {
            foreach (var item in db)
            {
                if (item.Name.Contains("(Banco Selecionado)"))
                {
                    item.Name = item.Name.Remove(item.Name.Length - 20);
                }
            }
        }


    }
}
