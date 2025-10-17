using Shell;
using System.Net.NetworkInformation;
using System.Net;

namespace Shell.Commands;

public class PingCommand : ICommand
{
  public string Name => "ping";
  public string Description => "Ping a host to test network connectivity";
  public string[] Aliases => new[] { "ping" };

  public void Execute(string[] args)
  {
    if (args.Length < 2)
    {
      Logger.Warning("Usage: ping <hostname or IP address>");
      return;
    }

    var target = args[1];
    const int pingCount = 5;
    const int timeout = 5000; // 5 seconds

    Logger.Info($"Pinging {target} with 32 bytes of data:");
    Logger.NewLine();

    var ping = new Ping();
    var results = new List<PingReply?>();
    var successfulPings = new List<long>();

    try
    {
      // Perform 5 pings
      for (int i = 0; i < pingCount; i++)
      {
        try
        {
          var reply = ping.Send(target, timeout);
          results.Add(reply);

          if (reply.Status == IPStatus.Success)
          {
            successfulPings.Add(reply.RoundtripTime);
            Logger.Info($"Reply from {reply.Address}: bytes=32 time={reply.RoundtripTime}ms TTL={reply.Options?.Ttl ?? 0}");
          }
          else
          {
            Logger.Warning($"Request timed out or failed: {GetStatusMessage(reply.Status)}");
          }
        }
        catch (PingException ex)
        {
          Logger.Error($"Ping failed: {ex.Message}");
          results.Add(null);
        }
        catch (Exception ex)
        {
          Logger.Error($"Unexpected error: {ex.Message}");
          results.Add(null);
        }

        // Small delay between pings (except for the last one)
        if (i < pingCount - 1)
        {
          Thread.Sleep(1000); // 1 second delay
        }
      }

      // Display statistics
      DisplayStatistics(target, results, successfulPings);
    }
    catch (Exception ex)
    {
      Logger.Error($"Failed to ping {target}: {ex.Message}");
    }
    finally
    {
      ping?.Dispose();
    }
  }

  private static string GetStatusMessage(IPStatus status)
  {
    return status switch
    {
      IPStatus.TimedOut => "Request timed out",
      IPStatus.DestinationHostUnreachable => "Destination host unreachable",
      IPStatus.DestinationNetworkUnreachable => "Destination network unreachable",
      IPStatus.DestinationUnreachable => "Destination unreachable",
      IPStatus.DestinationProtocolUnreachable => "Destination protocol unreachable",
      IPStatus.DestinationPortUnreachable => "Destination port unreachable",
      IPStatus.NoResources => "No resources available",
      IPStatus.BadOption => "Bad option",
      IPStatus.HardwareError => "Hardware error",
      IPStatus.PacketTooBig => "Packet too big",
      IPStatus.TtlExpired => "TTL expired",
      IPStatus.TtlReassemblyTimeExceeded => "TTL reassembly time exceeded",
      IPStatus.ParameterProblem => "Parameter problem",
      IPStatus.SourceQuench => "Source quench",
      IPStatus.BadDestination => "Bad destination",
      IPStatus.TimeExceeded => "Time exceeded",
      IPStatus.BadHeader => "Bad header",
      IPStatus.UnrecognizedNextHeader => "Unrecognized next header",
      IPStatus.IcmpError => "ICMP error",
      IPStatus.DestinationScopeMismatch => "Destination scope mismatch",
      IPStatus.Unknown => "Unknown error",
      _ => status.ToString()
    };
  }

  private static void DisplayStatistics(string target, List<PingReply?> results, List<long> successfulPings)
  {
    Logger.NewLine();
    Logger.Info($"Ping statistics for {target}:");

    var sent = results.Count;
    var received = results.Count(r => r?.Status == IPStatus.Success);
    var lost = sent - received;
    var lossPercentage = sent > 0 ? (lost * 100.0 / sent) : 0;

    Logger.Info($"    Packets: Sent = {sent}, Received = {received}, Lost = {lost} ({(int)lossPercentage}% loss)");

    if (successfulPings.Count > 0)
    {
      var minTime = successfulPings.Min();
      var maxTime = successfulPings.Max();
      var avgTime = (long)successfulPings.Average();

      Logger.NewLine();
      Logger.Info("Approximate round trip times in milliseconds:");
      Logger.Info($"    Minimum = {minTime}ms, Maximum = {maxTime}ms, Average = {avgTime}ms");
    }
  }
}
