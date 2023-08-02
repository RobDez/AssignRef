// import node module libraries
import React, { Fragment, useEffect, useState, useCallback } from "react";
import PropTypes from "prop-types";
import {
  Col,
  Row,
  Card,
  Nav,
  Tab,
  Table,
  Form,
  FormSelect,
} from "bootstrap";
// import toastr from "toastr";
import Pagination from "rc-pagination";
import Swal from "sweetalert2";
import "sweetalert2/src/sweetalert2.scss";
import "rc-pagination/assets/index.css";
import "./game.css";
import debug from "debug";
import toastr from "toastr";
// import sub components
import SingleGame from "./SingleGame";
import TitleHeader from "components/general/TitleHeader";
import seasonService from "services/seasonService";
import gameService from "services/gameService";
import teamService from "services/teamService";

const _logger = debug.extend("GamesTable");

function GamesTable({ currentUser }) {
  const conferenceId = currentUser.conferenceId;

  const [pageData, setPageData] = useState({
    currentSeason: null,
    currentWeek: null,
    seasons: [],
    seasonComponents: [],
    seasonSelector: [],
    games: [],
    gameComponents: [],
    weekComponents: [],
    teamOptionComponents: [],
    currentSeasonGames: [],
    currentTeam: "",
    filteredGames: [],
    teamsCurrentPage: 1,
    weeksCurrentPage: 1,
    currentPage: 1,
    pageSize: 10,
    totalPages: 0,
  });

  useEffect(() => {
    seasonService
      .getByConferenceId(conferenceId)
      .then(getSeasonSuccess)
      .catch(getSeasonError);
  }, [conferenceId]);

  useEffect(() => {
    if (pageData.currentSeason) {
      seasonService
        .getSeasonById(pageData.currentSeason)
        .then(onGetSeasonByIdSuccess)
        .catch(onGetSeasonByIdError);

      teamService
        .getByConferenceId(conferenceId)
        .then(onGetAllSuccess)
        .catch(onGetAllError);
    }
  }, [pageData.currentSeason, pageData.currentPage]);

  useEffect(() => {
    if (pageData.currentWeek && pageData.currentTeam === null) {
      gameService
        .GetBySeasonIdAndWeekPaginated(
          pageData.weeksCurrentPage - 1,
          pageData.pageSize,
          pageData.currentSeason,
          pageData.currentWeek
        )
        .then(onGetBySeasonIdAndWeekPaginatedSuccess)
        .catch(onGetBySeasonIdAndWeekPaginatedError);
    }
  }, [pageData.currentWeek]);

  useEffect(() => {
    if (pageData.currentSeason) {
      gameService
        .GetBySeasonWeekAndTeam(
          pageData.currentPage - 1,
          pageData.pageSize,
          pageData.currentSeason,
          currentUser.conferenceId,
          pageData.currentWeek,
          pageData.currentTeam
        )
        .then(onGetBySeasonWeekAndTeamSuccess)
        .catch(onGetBySeasonWeekAndTeamError);
    }
  }, [
    pageData.currentTeam,
    pageData.currentWeek,
    pageData.currentSeason,
    pageData.currentPage,
  ]);

  const onGetBySeasonWeekAndTeamSuccess = (response) => {
    _logger("API:::", response);
    setPageData((prevState) => {
      let pageD = { ...prevState };
      pageD.games = response.item.pagedItems;
      pageD.gameComponents = response.item.pagedItems.map(mapGames);
      pageD.totalPages = response.item.totalCount;
      return pageD;
    });
  };

  const onGetBySeasonWeekAndTeamError = (error) => {
    _logger(error);
    setPageData((prevState) => {
      let pageD = { ...prevState };
      pageD.gameComponents = [];
      return pageD;
    });
  };

  const onGetAllSuccess = (response) => {
    _logger("this is the repsonse", response.items[0].name);

    setPageData((prevState) => {
      let pageD = { ...prevState };
      pageD.teamOptionComponents = response.items.map(mapTeamDropDown);
      return pageD;
    });
  };
  const onGetAllError = (error) => {
    _logger(error);
  };

  const onGetSeasonByIdSuccess = (response) => {
    setPageData((prevState) => {
      let pageD = { ...prevState };
      pageD.weekComponents = SeasonWeekOptions(response.item.weeks);
      return pageD;
    });
  };

  const onWeekDropDownSelect = (key) => {
    setPageData((prevState) => {
      let pageD = { ...prevState };
      pageD.currentWeek = key.target?.value;
      return pageD;
    });
  };
  const onGetBySeasonIdAndWeekPaginatedSuccess = (response) => {
    setPageData((prevState) => {
      let pageD = { ...prevState };
      pageD.games = response.item.pagedItems;
      pageD.gameComponents = response.item.pagedItems.map(mapGames);
      pageD.totalPages = response.item.totalCount;
      return pageD;
    });
  };

  const onGetBySeasonIdAndWeekPaginatedError = (error) => {
    _logger(error);
    toastr["warning"]("Sorry there are no games for this week");
  };

  const onGetSeasonByIdError = (error) => {
    _logger(error);
    toastr["error"]("Error occured please try again.");
  };

  const SeasonWeekOptions = (numberOfWeeks) => {
    let result = [];

    for (let i = 1; i < numberOfWeeks + 1; i++) {
      result.push(
        <option value={i} key={i} id={i}>
          {i}
        </option>
      );
    }

    return result;
  };

  const getSeasonSuccess = (response) => {
    _logger(response);
    if (response.items.length > 0) {
      gameService
        .GetBySeasonIdPaginated(
          response.items[0]?.id,
          pageData.currentPage - 1,
          pageData.pageSize
        )
        .then((data) =>
          setPageData((prevState) => {
            const pageD = { ...prevState };
            pageD.currentSeason = response.items[0].id;
            pageD.seasons = response.items;
            pageD.seasonComponents = response.items.map(mapSeason);
            pageD.seasonSelector = response.items.map(seasonMapper);
            pageD.games = data.item.pagedItems;
            pageD.gameComponents = data.item.pagedItems.map(mapGames);

            pageD.totalPages = data.item.totalCount;
            return pageD;
          })
        )
        .catch(getSeasonError);
    }
  };

  const pageOnChange = (page) => {
    setPageData((prevState) => {
      const pageD = { ...prevState };
      pageD.currentPage = page;
      return pageD;
    });
  };

  const getSeasonError = () => {
    setPageData((prevState) => {
      const pageD = { ...prevState };
      pageD.currentPage = 1;
      pageD.games = [];
      pageD.gameComponents = [];

      return pageD;
    });
  };

  const onDeletdRequested = useCallback((theGame) => {
    Swal.fire({
      title: "You sure want to delete this Game?",
      showDenyButton: true,
      confirmButtonText: "Yes",
      denyButtonText: `No`,
    }).then((result) => {
      if (result.isConfirmed) {
        const handler = onDeleteGameSuccess(theGame.id);
        gameService
          .DeleteById(theGame.id)
          .then(handler)
          .catch(onDeleteGameError);
      } else {
        Swal.fire("Changes are not save");
      }
    });
  }, []);

  const onDeleteGameError = () => {
    toastr["error"]("An error occured. Please try again.");
  };

  const onDeleteGameSuccess = (responese) => {
    Swal.fire("Successfully Deleted!");
    const filterGames = (target) => {
      return target.id !== responese;
    };
    setPageData((prevState) => {
      const pageD = { ...prevState };
      pageD.games = prevState.games.filter(filterGames);
      pageD.gameComponents = prevState.games.filter(filterGames).map(mapGames);
      return pageD;
    });
  };

  const onDropDownSelect = (key) => {
    _logger(key.target.value);

    setPageData((prevState) => {
      const pageD = { ...prevState };
      pageD.currentSeason = parseInt(key.target.value);
      pageD.currentPage = 1;
      return pageD;
    });
  };

  const onTeamDropDownSelect = (key) => {
    setPageData((prevState) => {
      let pageD = { ...prevState };
      pageD.currentTeam = key.target.value;

      return pageD;
    });
  };

  const mapTeamDropDown = (team) => {
    return (
      <option value={team.id} key={team.id}>
        {team.name}
      </option>
    );
  };

  const mapSeason = (season) => {
    return (
      <Nav.Item key={season.id}>
        <Nav.Link eventKey={season.id} className="mb-sm-3 mb-md-0">
          {season.name}
        </Nav.Link>
      </Nav.Item>
    );
  };

  const mapGames = (game) => {
    return (
      <SingleGame
        game={game}
        key={"game" + game.id}
        onDeletdRequested={onDeletdRequested}
      />
    );
  };

  const seasonMapper = (item) => {
    return (
      <option value={item.id} key={item.id}>
        {item.name}
      </option>
    );
  };

  return (
    <Fragment>
      <TitleHeader
        title="Games"
        buttonText="New Game"
        buttonLink="/games/new"
      />
      <Row>
        <Col lg={12} md={12} sm={12}>
          <Tab.Container defaultActiveKey="all">
            <Card>
              <Card.Header className="border-bottom-0 p-0 bg-white">
                <div className="d-flex flex-row-reverse">
                  <div className=" p-2 col-md-2">
                    <Form.Select
                      name="season"
                      className="form-select-sm text-primary"
                      onChange={onDropDownSelect}
                    >
                      {pageData.seasonSelector}
                    </Form.Select>
                  </div>
                  <div className="p-2 col-md-2">
                    <FormSelect
                      name="weeks"
                      className="form-select-sm text-primary"
                      onChange={onWeekDropDownSelect}
                    >
                      <option value={""}>Select Week</option>
                      {pageData.weekComponents}
                    </FormSelect>
                  </div>
                  <div className="p-2 col-md-2">
                    <Form.Select
                      name="teamDropdown"
                      className="form-select-sm text-primary"
                      onChange={onTeamDropDownSelect}
                    >
                      <option value={""}>Select Team</option>
                      {pageData.teamOptionComponents}
                    </Form.Select>
                  </div>
                </div>
              </Card.Header>
              <Card.Body className="p-0">
                <div className="table-responsive border-0 overflow-y-hidden">
                  <Table hover className="text-center">
                    <thead className="table-light">
                      <tr>
                        <th className="px-2">Game #</th>
                        <th>Date and Time</th>
                        <th>Home Team</th>
                        <th>Visting Team</th>
                        <th className="px-2">Conference</th>
                        <th>Location</th>
                        <th className="px-2">Week</th>
                        <th className="px-2">Season</th>
                        <th className="px-2">Game Status</th>
                        <th className="px-2">Delete</th>
                      </tr>
                    </thead>

                    {pageData.gameComponents?.length > 0 ? (
                      pageData.gameComponents
                    ) : (
                      <tbody>
                        <td colSpan={2} className="p-4 mx-0">
                          No games found
                        </td>
                      </tbody>
                    )}
                  </Table>
                </div>
                <div className="text-center my-3">
                  <Pagination
                    onChange={pageOnChange}
                    current={pageData.currentPage}
                    total={pageData.totalPages}
                    pageSize={pageData.pageSize}
                  />
                </div>
              </Card.Body>
            </Card>
          </Tab.Container>
        </Col>
      </Row>
    </Fragment>
  );
}

GamesTable.propTypes = {
  currentUser: PropTypes.shape({
    conferenceId: PropTypes.number.isRequired,
  }).isRequired,
};

export default GamesTable;
