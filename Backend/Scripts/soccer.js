$(document).ready(function () {
    var dateFormated = moment($('#datepicker input').val(), 'DD/MM/YYYY').format('YYYY/MM/DD');
    $('#datepicker input').val(dateFormated);
    $('#datepicker').datetimepicker({ format: 'YYYY/MM/DD' });
    $('#timepicker').datetimepicker({ format: 'LT' });

    $("#LeagueId").change(function () {
        $("#TeamId").empty();
        $.ajax({
            type: 'POST',
            url: Url,
            dataType: 'json',
            data: { leagueId: $("#LeagueId").val() },
            success: function (teams) {
                $.each(teams, function (i, team) {
                    $("#TeamId").append('<option value="'
                     + team.TeamId + '">'
                     + team.Name + '</option>');
                });
            },
            error: function (ex) {
                alert('Failed to retrieve teams.' + ex);
            }
        });

        return false;
    });

    $("#LocalLeagueId").change(function () {
        $("#LocalId").empty();
        $.ajax({
            type: 'POST',
            url: Url,
            dataType: 'json',
            data: { leagueId: $("#LocalLeagueId").val() },
            success: function (teams) {
                $.each(teams, function (i, team) {
                    $("#LocalId").append('<option value="'
                     + team.TeamId + '">'
                     + team.Name + '</option>');
                });
            },
            error: function (ex) {
                alert('Failed to retrieve teams.' + ex);
            }
        });

        return false;
    });

    $("#VisitorLeagueId").change(function () {
        $("#VisitorId").empty();
        $.ajax({
            type: 'POST',
            url: Url,
            dataType: 'json',
            data: { leagueId: $("#VisitorLeagueId").val() },
            success: function (teams) {
                $.each(teams, function (i, team) {
                    $("#VisitorId").append('<option value="'
                     + team.TeamId + '">'
                     + team.Name + '</option>');
                });
            },
            error: function (ex) {
                alert('Failed to retrieve teams.' + ex);
            }
        });

        return false;
    });

    $("#FavoriteLeagueId").change(function () {
        $("#FavoriteTeamId").empty();
        $.ajax({
            type: 'POST',
            url: Url,
            dataType: 'json',
            data: { leagueId: $("#FavoriteLeagueId").val() },
            success: function (teams) {
                $.each(teams, function (i, team) {
                    $("#FavoriteTeamId").append('<option value="'
                     + team.TeamId + '">'
                     + team.Name + '</option>');
                });
            },
            error: function (ex) {
                alert('Failed to retrieve teams.' + ex);
            }
        });

        return false;
    });
});
