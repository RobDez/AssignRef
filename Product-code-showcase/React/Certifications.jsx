import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Card, Row, Table, Form } from "react-bootstrap";
import { CardBody, Col } from "reactstrap";
import debug from "sabio-debug";
import Pagination from "rc-pagination";
import "rc-pagination/assets/index.css";
import locale from "rc-pagination/lib/locale/en_US";
import Swal from "sweetalert2";
import TitleHeader from "components/general/TitleHeader";
import MappingCertification from "./MapCertification";
import certificationService from "services/certificationService";
import seasonService from "services/seasonService";

import PropTypes from "prop-types";
import conferencesService from "services/conferenceService";
import toastr from "toastr";

function Certifications(props) {
  const navigate = useNavigate();
  const _logger = debug.extend("CertificationForm");
  const isAdmin = props.currentUser.roles.some((role) => role === "Admin");
  const conferenceId = props.currentUser.conferenceId;
  const [pageData, setPageData] = useState({
    allCertifications: [],
    pagedCertifications: [],
    arrayFiltered: [],
    certificationsComponents: [],
    filteredComponents: [],
    pageIndex: 0,
    pageSize: 6,
    current: 1,
    total: 1,
    season: 0,
    query: "",
    property: 0,
  });

  const [renderingData, setRenderingData] = useState({
    conferenceId: props.currentUser.conferenceId,
    seasonsOptions: "",
    showAllCertifications: true,
    showFilteredCertifications: false,
  });

  useEffect(() => {
    setPageData((prevState) => ({ ...prevState, current: 1 }));
    certificationService
      .getByConferenceId(0, pageData.pageSize, conferenceId)
      .then(onGetPagedCertificationsSuccess)
      .catch(onGetCertificationsError);
    certificationService
      .getAll(0, 100)
      .then(onGetAllCertificationsSuccess)
      .catch(onGetCertificationsError);

    if (isAdmin) {
      conferencesService
        .getAll()
        .then(onGetAllConferencesSuccess)
        .catch(onGetAllConferencesError);
      seasonService
        .getByConferenceId(conferenceId)
        .then(onGetSeasonsSuccess)
        .catch(onGetSeasonsError);
    } else {
      seasonService
        .getByConferenceId(conferenceId)
        .then(onGetSeasonsSuccess)
        .catch(onGetSeasonsError);
    }
  }, [conferenceId]);

  useEffect(() => {
    certificationService
      .getByConferenceId(pageData.pageIndex, pageData.pageSize, conferenceId)
      .then(onGetPagedCertificationsSuccess)
      .catch(onGetCertificationsError);
  }, [pageData.pageIndex, conferenceId]);

  const onGetPagedCertificationsSuccess = (data) => {
    const certificationsArray = data.item.pagedItems;

    setPageData((prevState) => {
      const newState = { ...prevState };

      newState.pagedCertifications = certificationsArray;
      newState.certificationsComponents =
        certificationsArray.map(mapCertification);
      newState.total = data.item.totalCount;
      return newState;
    });
  };

  const onGetAllCertificationsSuccess = (data) => {
    const certificationsArray = data.item.pagedItems;

    setPageData((prevState) => {
      const newState = { ...prevState };
      newState.allCertifications = certificationsArray.filter(
        (certObject) => certObject.season.conference.id === Number(conferenceId)
      );

      return newState;
    });
  };

  const onGetSeasonsSuccess = (data) => {
    const seasonsArray = data.items;

    if (seasonsArray.length === 0) {
      Swal.fire({
        title: "Certifications not available",
        html: "There are no certifications for this specific conference",
        icon: "warning",
        allowOutsideClick: false,
      });

      toastr.info("No Seasons Found for Conference ", "Warning");
    }

    const mappedArray = seasonsArray.map(mappingOptions);
    setRenderingData((prevState) => ({
      ...prevState,
      seasonsOptions: mappedArray,
    }));
  };

  const mapCertification = (aCertificationObject) => {
    return (
      <MappingCertification
        certification={aCertificationObject}
        key={"cert-" + aCertificationObject.id}
        onEdit={onEditCerfication}
        onDelete={onDeleteCertification}
        onResult={onResultCertification}
      ></MappingCertification>
    );
  };

  const onResultCertification = (aCertification) => {
    const targetPageResult = `/certification/${aCertification.id}/results`;
    navigate(targetPageResult);
  };

  const onEditCerfication = (aCertification) => {
    const targetPageEdit = `/certifications/${aCertification.id}`;
    const stateToBeSent = {
      type: "CERTIFICATION_EDIT",
      payload: aCertification,
    };

    navigate(targetPageEdit, { state: stateToBeSent });
  };

  const onDeleteCertification = (aCertification) => {
    const idToDelete = aCertification.id;

    Swal.fire({
      title: "Deleting Certification",
      html: `<h3>Do you want to delete Certification: ${idToDelete}?</h3>`,
      icon: "warning",
      showCancelButton: true,
      cancelButtonColor: "#3085d6",
      confirmButtonColor: "#d33",
      confirmButtonText: "Delete Certification",
    }).then((result) => {
      if (result.isConfirmed) {
        const handler = getDeleteSuccessHandler(idToDelete);
        certificationService
          .delete(idToDelete)
          .then(handler)
          .catch(onDeleteCertificationError);

        Swal.fire({
          title: "Certification Deleted",
          html: `<h3>Certification is no longer active.</h3>`,
          icon: "success",
          confirmButtonColor: "#28a745",
        });
      }
    });
  };

  const getDeleteSuccessHandler = (idToDelete) => {
    return () => {
      setPageData((prevState) => {
        const pageData = { ...prevState };
        pageData.pagedCertifications = [...pageData.pagedCertifications];

        const indexOfCertification = pageData.pagedCertifications.findIndex(
          (cert) => {
            let result = false;

            if (cert.id === idToDelete) {
              result = true;
            }

            return result;
          }
        );

        if (indexOfCertification >= 0) {
          pageData.pagedCertifications.splice(indexOfCertification, 1);
          pageData.certificationsComponents =
            pageData.pagedCertifications.map(mapCertification);
        }

        return pageData;
      });
    };
  };

  const onPaginationChange = (page) => {
    setPageData((prevState) => {
      const newState = { ...prevState };
      newState.current = page;
      newState.pageIndex = page - 1;

      return newState;
    });
  };

  const onSeasonSelect = (event) => {
    const selectedSeason = event.target.value;

    _logger("season", selectedSeason);
    setPageData((prevState) => ({
      ...prevState,
      season: selectedSeason,
      query: "",
      property: 0,
    }));

    if (selectedSeason === "0") {
      setRenderingData((prevState) => ({
        ...prevState,
        showAllCertifications: true,
        showFilteredCertifications: false,
      }));
    } else {
      setRenderingData((prevState) => ({
        ...prevState,
        showAllCertifications: false,
        showFilteredCertifications: true,
      }));
      setPageData((prevState) => {
        const newState = { ...prevState };
        newState.arrayFiltered = newState.allCertifications.filter(
          (certObject) => certObject.season.id === Number(selectedSeason)
        );
        newState.filteredComponents =
          newState.arrayFiltered.map(mapCertification);

        return newState;
      });
    }
  };

  const onSearchCertification = (event) => {
    const certificationSearch = event.target.value;

    setPageData((prevState) => ({
      ...prevState,
      season: 0,
      query: certificationSearch,
      property: 0,
    }));

    if (certificationSearch === "") {
      setRenderingData((prevState) => ({
        ...prevState,
        showAllCertifications: true,
        showFilteredCertifications: false,
      }));
    } else {
      setRenderingData((prevState) => ({
        ...prevState,
        showAllCertifications: false,
        showFilteredCertifications: true,
      }));

      setPageData((prevState) => {
        const newState = { ...prevState };
        newState.arrayFiltered = newState.allCertifications.filter(
          (certObject) =>
            certObject.name
              .toLowerCase()
              .indexOf(certificationSearch.toLowerCase()) >= 0
        );
        newState.filteredComponents =
          newState.arrayFiltered.map(mapCertification);

        return newState;
      });
    }
  };

  const onFilterByProperty = (event) => {
    const selectedProperty = event.target.value;

    setPageData((prevState) => ({
      ...prevState,
      season: 0,
      query: "",
      property: selectedProperty,
    }));

    if (selectedProperty === "0") {
      setRenderingData((prevState) => ({
        ...prevState,
        showAllCertifications: true,
        showFilteredCertifications: false,
      }));
    } else {
      setRenderingData((prevState) => ({
        ...prevState,
        showAllCertifications: false,
        showFilteredCertifications: true,
      }));

      switch (Number(selectedProperty)) {
        case 1:
          filterHandler("isPhysicalRequired");
          break;
        case 2:
          filterHandler("isBackgroundCheckRequired");
          break;
        case 3:
          filterHandler("isTestRequired");
          break;
        case 4:
          filterHandler("isFitnessTestRequired");
          break;
        case 5:
          filterHandler("isClinicRequired");
          break;
        default:
          break;
      }
    }
  };

  const filterHandler = (propertyToFilter) => {
    setPageData((prevState) => {
      const newState = { ...prevState };
      newState.arrayFiltered = newState.allCertifications.filter(
        (certObject) => certObject[propertyToFilter] === true
      );
      newState.filteredComponents =
        newState.arrayFiltered.map(mapCertification);

      return newState;
    });
  };

  const mappingOptions = (anOption) => {
    return (
      <option value={anOption.id} key={"option" + anOption.id}>
        {" "}
        {anOption.name}
      </option>
    );
  };

  const onGetCertificationsError = () => {
    toastr.error("Certifications not Rendered", "Error");

    setPageData((prevState) => {
      const newState = { ...prevState };

      newState.pagedCertifications = "";
      newState.certificationsComponents = "";
      newState.total = 0;
      return newState;
    });
  };

  const onDeleteCertificationError = () => {
    toastr.error("Certification not Deleted", "Error");
  };

  const onGetSeasonsError = () => {
    toastr.error("Seasons not Rendered", "Error");
  };

  const onGetAllConferencesSuccess = (data) => {
    const conferencesArray = data.items;
    const mappedArray = conferencesArray.map(mappingOptions);
    setRenderingData((prevState) => ({
      ...prevState,
      conferences: mappedArray,
    }));
  };

  const selectingConferenceId = (event) => {
    const selectedConference = event.target.value;
    _logger("Changing conference", selectedConference);

    setPageData((prevState) => ({
      ...prevState,
      season: "",
      query: "",
      property: 0,
    }));
    setRenderingData((prevState) => ({
      ...prevState,
      conferenceId: selectedConference,
      showAllCertifications: true,
      showFilteredCertifications: false,
    }));
  };

  const onGetAllConferencesError = () => {
    toastr.warning("No conferences found in the current Database", "Warning");
  };

  return (
    <React.Fragment>
      <TitleHeader
        title="Certifications"
        buttonText="Add Certification"
        buttonLink="/certifications/new"
      />
      <Row>
        <Col>
          <Card>
            <CardBody className="p-0">
              <Row className="p-3">
                <Col className="col-md-3">
                  <input
                    id="search"
                    type="search"
                    className="form-control"
                    placeholder="Name"
                    onChange={onSearchCertification}
                    value={pageData.query}
                  />
                </Col>
                <Col className="col-md-3">
                  {isAdmin && (
                    <>
                      <select
                        className="form-select mb-2 text-dark"
                        aria-label="Default select example"
                        id="conferenceId"
                        name="conferenceId"
                        onChange={selectingConferenceId}
                      >
                        <option value={props.currentUser.conferenceId}>
                          Current Conference
                        </option>
                        {renderingData.conferences}
                      </select>
                    </>
                  )}
                </Col>
                <Col className="col-md-3">
                  <Form.Select
                    id="filterBy"
                    name="filterBy"
                    className="text-dark"
                    onChange={onFilterByProperty}
                    value={pageData.property}
                  >
                    <option value="0"> Filter By</option>
                    <option value="1"> Physical Test Required</option>
                    <option value="2"> Background Check Required</option>
                    <option value="3"> Written Test Required</option>
                    <option value="4"> Fitness Test Required</option>
                    <option value="5"> Clinic Test Required</option>
                  </Form.Select>
                </Col>
                <Col className="col-md-3">
                  <Form.Select
                    id="seasonSelect"
                    name="seasonSelect"
                    className="text-dark"
                    onChange={onSeasonSelect}
                    value={pageData.season}
                  >
                    <option value="0">All Seasons</option>
                    {renderingData.seasonsOptions}
                  </Form.Select>
                </Col>
              </Row>
              <div className="table-responsive">
                <Table className="table table-stripped table-hover">
                  <thead className="table-light">
                    <tr role="row" className="text-center align-middle">
                      <th colSpan="1" role="columnheader">
                        Id
                      </th>
                      <th colSpan="1" role="columnheader">
                        Conference
                      </th>
                      <th colSpan="1" role="columnheader">
                        Certification Name
                      </th>
                      <th colSpan="1" role="columnheader">
                        Season
                      </th>
                      <th colSpan="1" role="columnheader">
                        Physical Test
                      </th>
                      <th colSpan="1" role="columnheader">
                        Background Check
                      </th>
                      <th colSpan="1" role="columnheader">
                        Written Test
                      </th>
                      <th colSpan="1" role="columnheader">
                        Fitness Test
                      </th>
                      <th colSpan="1" role="columnheader">
                        Clinic Test
                      </th>
                      <th colSpan="1" role="columnheader">
                        Due Date
                      </th>
                      <th colSpan="1" role="columnheader" />
                    </tr>
                  </thead>
                  {renderingData.showAllCertifications &&
                    pageData.certificationsComponents}
                  {renderingData.showFilteredCertifications &&
                    pageData.filteredComponents}
                </Table>
                <div>
                  {renderingData.showAllCertifications && (
                    <Pagination
                      className="text-center m-3"
                      locale={locale}
                      onChange={onPaginationChange}
                      current={pageData.current}
                      total={pageData.total}
                      pageSize={pageData.pageSize}
                    />
                  )}
                  {renderingData.showFilteredCertifications && (
                    <Pagination
                      className="text-center m-3"
                      locale={locale}
                      pageSize={100}
                    />
                  )}
                </div>
              </div>
            </CardBody>
          </Card>
        </Col>
      </Row>

      <Row></Row>
    </React.Fragment>
  );
}

Certifications.propTypes = {
  currentUser: PropTypes.shape({
    conferenceId: PropTypes.number.isRequired,
    roles: PropTypes.arrayOf(PropTypes.string).isRequired,
    name: PropTypes.string.isRequired,
    isLoggedIn: PropTypes.bool.isRequired,
    id: PropTypes.number.isRequired,
    email: PropTypes.string.isRequired,
    avatarUrl: PropTypes.string.isRequired,
  }).isRequired,
};

export default Certifications;
