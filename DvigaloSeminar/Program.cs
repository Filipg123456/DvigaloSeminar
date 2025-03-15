using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class Dvigalo
{
    public int Id { get; }
    public int TrenutnoNadstropje { get; private set; } = 0;
    private List<int> CiljnaNadstropja = new List<int>();
    private Random random = new Random();
    public bool VrataOdprta { get; private set; } = false;
    public int Potniki { get; private set; } = 0;
    private const int MaksPotniki = 10;
    public int Smer { get; private set; } = 0; // -1 dol, 1 gor, 0 stoji

    public Dvigalo(int id) => Id = id;

    public void DodajZahtevo(int nadstropje)
    {
        if (!CiljnaNadstropja.Contains(nadstropje))
        {
            CiljnaNadstropja.Add(nadstropje);
            CiljnaNadstropja.Sort();
        }
    }

    public void Premakni()
    {
        if (CiljnaNadstropja.Count > 0)
        {
            int cilj = CiljnaNadstropja[0];
            Smer = Math.Sign(cilj - TrenutnoNadstropje);
            TrenutnoNadstropje += Smer;
            Console.WriteLine($"Dvigalo {Id} je na nadstropju {TrenutnoNadstropje}");

            if (TrenutnoNadstropje == cilj)
            {
                CiljnaNadstropja.RemoveAt(0);
                OdpriVrata();
                UpravljajPotnike();
                ZapriVrata();
            }
        }
        else
        {
            Smer = 0; // Če ni več ciljev, se dvigalo ustavi
        }
    }

    private void OdpriVrata()
    {
        VrataOdprta = true;
        Console.WriteLine($"Dvigalo {Id} odpira vrata na nadstropju {TrenutnoNadstropje}");
    }

    private void ZapriVrata()
    {
        VrataOdprta = false;
        Console.WriteLine($"Dvigalo {Id} zapira vrata.");
    }

    private void UpravljajPotnike()
    {
        int izstopajo = random.Next(0, Potniki + 1);
        Potniki -= izstopajo;
        Console.WriteLine($"{izstopajo} potnikov je izstopilo iz dvigala {Id}.");

        int vstopajo = random.Next(0, MaksPotniki - Potniki + 1);
        Potniki += vstopajo;
        Console.WriteLine($"{vstopajo} potnikov je vstopilo v dvigalo {Id}. Trenutno {Potniki} potnikov v dvigalu.");
    }
}

class SistemDvigal
{
    private List<Dvigalo> dvigala;

    public SistemDvigal(int stDvigal)
    {
        dvigala = Enumerable.Range(1, stDvigal).Select(id => new Dvigalo(id)).ToList();
    }

    public void DodajZahtevo(int zacetno, int ciljno)
    {
        var dvigalo = dvigala
            .Where(d => d.Smer == 0 || (d.Smer == 1 && zacetno >= d.TrenutnoNadstropje) || (d.Smer == -1 && zacetno <= d.TrenutnoNadstropje))
            .OrderBy(d => Math.Abs(d.TrenutnoNadstropje - zacetno))
            .FirstOrDefault() ?? dvigala.OrderBy(d => Math.Abs(d.TrenutnoNadstropje - zacetno)).First();

        dvigalo.DodajZahtevo(zacetno);
        dvigalo.DodajZahtevo(ciljno);
    }

    public void Simuliraj()
    {
        while (true)
        {
            dvigala.ForEach(d => d.Premakni());
            Thread.Sleep(1000);
        }
    }
}

class Program
{
    static void Main()
    {
        var sistem = new SistemDvigal(3);
        Thread nit = new Thread(sistem.Simuliraj) { IsBackground = true };
        nit.Start();

        while (true)
        {
            Console.Write("Začetno nadstropje (0-10): ");
            if (!int.TryParse(Console.ReadLine(), out int zacetno) || zacetno < 0 || zacetno > 10)
                continue;

            Console.Write("Ciljno nadstropje (0-10): ");
            if (!int.TryParse(Console.ReadLine(), out int ciljno) || ciljno < 0 || ciljno > 10)
                continue;

            sistem.DodajZahtevo(zacetno, ciljno);
        }
    }
}
