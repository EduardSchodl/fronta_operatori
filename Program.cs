
/**
 * Prichozi hovor do call centra
 */
class IncomingCall
{
    /** Volajici cislo */
    public int callingNumber;
    /** Cas kdy hovor prisel (v sekundach od zacatku smeny) */
    public int time;
}


/// <summary>
/// Třída reprezentující operátora
/// </summary>
class FreeOperator
{
    public string name;
    public int time;

    /// <summary>
    /// Kontruktor třídy <b>FreeOperator</b>
    /// </summary>
    /// <param name="name">Jméno operátora</param>
    /// <param name="time">Čas přihlášení operátora</param>
    public FreeOperator(string name, int time)
    {
        this.name = name;
        this.time = time;
    }
}

/**
 * Odbavovani prichozich hovoru pomoci operatoru
 */
class Dispatcher
{ 
    /** Fronta prichozich hovoru */
    private Queue<IncomingCall> callerQueue;
    /** Fronta operatoru */
    private Queue<FreeOperator> operatorQueue;

    /**
	 * Vytvori novou instanci s prazdnymi frontami
	 */
    public Dispatcher()
    {
        this.callerQueue = new Queue<IncomingCall>();
        this.operatorQueue = new Queue<FreeOperator>();
    }

    /**
	 * Zaradi prichozi hovor do fronty
	 * @param number telefonni cislo prichoziho hvoru
	 * @param time cas zacatku hovoru (v sekundach od zacatku smeny)
	 */
    public void Call(int number, int time)
    {
        IncomingCall call = new IncomingCall();
        call.callingNumber = number;
        call.time = time;
        callerQueue.Add(call);
    }

    /**
	 * Zaradi volneho operatora do fronty
	 * @param name jmeno volneho operatora
	 * @param time cas zarazeni volneho operatora do fronty (v sekundach od zacatku smeny)
	 */
    public void FreeOperator(String name, int time)
    {
        operatorQueue.Add(new FreeOperator(name, time)); // operator name se time sekund od zacatku smeny prihlasil jako dostupny
    }

    /**
	 * Priradi nejdele cekajici hovor z fronty nejdele cekajicimu operatorovi z fronty
	 */
    public void DispatchCall()
    {
        if (callerQueue.IsEmpty() || operatorQueue.IsEmpty())
        {
            return;
        }

        AssignCall(callerQueue.Get(), operatorQueue.Get());
        callerQueue.removeFirst();
        operatorQueue.removeFirst();
    }

    /**
	 * Priradi zadany prichozi hovor zadanemu volnemu operatorovi 
	 * @param call prichozi hovor
	 * @param operator volny operator
	 */
    private void AssignCall(IncomingCall call, FreeOperator oper)
    {
        int operPause = Math.Max(0, call.time - oper.time);
        
        if(operPause > CallDispatching.longestPause)
        {
            CallDispatching.longestPause = operPause;
            CallDispatching.operatorName = oper.name;
            CallDispatching.callNumber = call.callingNumber;
        }

        string output = oper.name + " is answering call from +420 " + call.callingNumber + "\n " + "The caller has waited for " + Math.Max(0, oper.time - call.time) + " seconds.";
        //string output = oper.name + " is answering call from +420 " + call.callingNumber + "\n " + "The operator has waited for " + operPause + " seconds.";

        Console.WriteLine(output);

        CallDispatching.sw.WriteLine(output);
    }
}

public class CallDispatching
{
    public static FileStream fs = new FileStream("dispatchCall.txt", FileMode.Create);
    public static StreamWriter sw = new StreamWriter(fs);

    public static int longestPause = 0;
    public static string operatorName = "";
    public static int callNumber = 0;

    public static void Main(String[] args)
    {
        /*
        Dispatcher d = new Dispatcher();
        d.FreeOperator("Tonda", 0);
        d.DispatchCall();
        d.FreeOperator("Jarmila", 10);
        d.DispatchCall();
        d.Call(608123456, 15);
        d.DispatchCall();
        d.Call(723987654, 35);
        d.DispatchCall();
        d.Call(602112233, 45);
        d.DispatchCall();
        d.FreeOperator("Pepa", 62);
        d.DispatchCall();
        d.Call(608987654, 124);
        d.DispatchCall();
        d.FreeOperator("Tonda", 240);
        d.DispatchCall();
        */

        
        Dispatcher d = new Dispatcher();

        StreamReader sr = new StreamReader("CallCentrum.txt");

        
        string[] rec = new string[3];

        string line;
        while(!sr.EndOfStream)
        {
            line = sr.ReadLine();
            rec = line.Split(" ");

            switch (rec[0])
            {
                case "O":
                    d.FreeOperator(rec[2], Int32.Parse(rec[1]));
                    break;

                case "C":
                    d.Call(Int32.Parse(rec[2]), Int32.Parse(rec[1]));
                    break;
            }
            d.DispatchCall();
        }

        Console.WriteLine($"Nejdéle čekal operátor {operatorName} ({longestPause} sekund). Nakonec byl spojen s číslem {callNumber}.");
    }
}

/// <summary>
/// Generická třída představující prvek ve spojové struktuře
/// </summary>
/// <typeparam name="T">Typ prvku</typeparam>
class Link<T>
{
    public T data;
    public Link<T> next;
}

/// <summary>
/// Třída představující frontu
/// </summary>
/// <typeparam name="T">Typ fronty</typeparam>
class Queue<T>
{
    //První prvek ve frontě
    public Link<T> first;
    //Poslední prvek ve frontě
    public Link<T> last;

    /// <summary>
    /// Metoda pro přidání prvku do fronty.
    /// </summary>
    /// <param name="data">Přidávaný prvek</param>
    public void Add(T data)
    {
        Link<T> nl = new Link<T>();
        nl.data = data;

        if (first == null)
        {
            first = nl;
            last = nl;
        }
        else
        {
            last.next = nl;
            last = nl;
        }
    }

    /// <summary>
    /// Metoda vracející prvek na začátku fronty. Pokud je fronta prázdná vrátí se defaultní hodnota generického typu.
    /// </summary>
    /// <returns>Prvek na začátku neprázdné fronty, jinak defaultní hodnotu</returns>
    public T Get()
    {
        if (first != null)
        {
            return first.data;
        }
        return default(T);

    }

    /// <summary>
    /// Metoda z neprázdné fronty odstraní prvek na začátku. Pokud je prázdná, vypíše chybovou hlášku.
    /// </summary>
    public void removeFirst()
    {
        if (first != null)
        {
            first = first.next;
        }
        else
        {
            Console.WriteLine("Remove operator on empty queue. Probably error, continuing...");
        }
    }

    /// <summary>
    /// Metoda zkontroluje, zda je fronta prázdná.
    /// </summary>
    /// <returns>True, pokud je prázdná, jinak false</returns>
    public bool IsEmpty()
    {
        if (first == null)
        {
            return true;
        }
        return false;
    }
}