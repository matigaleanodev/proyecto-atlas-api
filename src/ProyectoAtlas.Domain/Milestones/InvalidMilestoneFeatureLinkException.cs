namespace ProyectoAtlas.Domain.Milestones;

public sealed class InvalidMilestoneFeatureLinkException(string message) : Exception(message);
